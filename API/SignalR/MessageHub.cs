using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class MessageHub: Hub
{

    public readonly IMessageRepository  _messageRepository;

    public readonly IUserRepository _userRepository;

    public readonly IMapper _mapper;

    public readonly IHubContext<PresenceHub> _hubContext;

    public readonly PresentTracker _presentTracker;

    public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository,
     IMapper mapper,IHubContext<PresenceHub> hubContext, PresentTracker presentTracker)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _hubContext = hubContext;
        _presentTracker = presentTracker;
        
    }
   

    public async Task SendMessage(CreateMessageDto createMessageDtoessageDto)
    {
        var username = Context.User.GetUserName();
        if (username == createMessageDtoessageDto.RecipientUsername.ToLower())
        {
            throw new HubException("You cannot send messages to yourself");
        }

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDtoessageDto.RecipientUsername);

        if (recipient == null) throw new HubException("User not found");

        var message= new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDtoessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);

        var group = await _messageRepository.GetMessageGroupConnection(groupName);

        if (group.Connections.Any(x => x.UserName == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await _presentTracker.GetConnectionForUsers(new string[] { sender.UserName, recipient.UserName });
            if (connections != null)
            {
                await _hubContext.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                new { username = sender.UserName, knownAs = sender.KnownAs });
            }
        }
       

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {
            
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }

      
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group=await AddGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);        
    
         await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var group=await RemoveFromGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }



    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<Group>AddGroup(string groupName)
    {
        var group = await _messageRepository.GetMessageGroupConnection(groupName);   

        var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());
        if (group == null)
        {
            group = new Group(groupName);
            _messageRepository.AddGroup(group);
        }
        group.Connections.Add(connection);

        if  (await _messageRepository.SaveAllAsync()) return group;

        throw new HubException("Failed to join group");
    }  

    private async Task<Group> RemoveFromGroup()
    {
        var group = await _messageRepository.GetMessageGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _messageRepository.RemoveConnection(connection);
        if (await _messageRepository.SaveAllAsync()) return group;

        throw new HubException("Failed to remove from group");
    }




}
