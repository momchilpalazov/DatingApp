using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Repository;

public class MessageRepository : IMessageRepository
{

    public readonly DaitingAppDbContext _context;

    public readonly IMessageRepository _messageRepository;

    public MessageRepository(DaitingAppDbContext context, IMessageRepository messageRepository)
    {
        _context = context;
        _messageRepository = messageRepository;
    }


    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessageAsync(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public Task<PagedList<MessageDto>> GetMessagesForUser()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
