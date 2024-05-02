using API.Interfaces;
using API.Repository;
using AutoMapper;

namespace API.Data;

public class UnitOfWork : IUnitOfWorkRepository
{

    public readonly DaitingAppDbContext _context;

    public readonly IMapper _mapper;

    public UnitOfWork(DaitingAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }




    public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

    public IUserRepository UserRepository => new UserRepository(_context, _mapper);

    public IlikeRepository LikeRepository => new LikeRepository(_context);  

    public async Task<bool> Complete()
    {
       return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}

