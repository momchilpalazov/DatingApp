using API.Interfaces;

namespace API;

public interface IUnitOfWorkRepository
{
    
    IMessageRepository MessageRepository { get; }
    IUserRepository UserRepository { get; }

    IlikeRepository LikeRepository { get; }
    Task<bool> Complete();

    bool HasChanges();

}
