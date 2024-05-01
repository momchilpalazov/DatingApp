using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;

public class MessageRepository : IMessageRepository
{

    public readonly DaitingAppDbContext _context;

    public readonly IMapper _mapper;

   

    public MessageRepository(DaitingAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
       
    }

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Message> GetMessageAsync(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<Group> GetMessageGroupConnection(string groupName)
    {
        return await _context.Groups.Include(c => c.Connections).FirstOrDefaultAsync(c => c.Name == groupName);
    }

    public async Task<Group> GetMessageGroupForConnection(string connectionId)
    {
        return await _context.Groups.Include(c => c.Connections)
        .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
        .FirstOrDefaultAsync();
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser( MessageParams messageParams)
    {
        var query = _context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false),
            "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username && u.SenderDeleted == false),
            _ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
        };

        var messages = query.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderUsername = m.Sender.UserName,
            RecipientUsername = m.Recipient.UserName,
            SenderPhotoUrl = m.Sender.Photos.FirstOrDefault(p => p.IsMain).Url,
            RecipientPhotoUrl = m.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url,
            Content = m.Content,
            DateRead = m.DateRead.Value,
            MessageSent = m.MessageSent
        });

        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }
 

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
    {
       var messages = await _context.Messages
       .Include(u => u.Sender).ThenInclude(p => p.Photos)
         .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(
            m => 
            m.Recipient.UserName == currentUserName && 
            m.RecipientDeleted == false && 
            m.Sender.UserName == recipientUserName|| 
            m.Recipient.UserName == recipientUserName && 
            m.Sender.UserName == currentUserName 
            && m.SenderDeleted == false
            )
            .OrderBy(m => m.MessageSent)     
            .ToListAsync();

        var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUserName).ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        return  _mapper.Map<IEnumerable<MessageDto>>(messages);           

    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
