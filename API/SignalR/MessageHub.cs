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

    public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
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

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {
            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }

      
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await _messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);
        if (_messageRepository.SaveAllAsync().Result)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }



    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

}
