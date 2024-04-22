using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;

public class LikeRepository : IlikeRepository
{

    private readonly DaitingAppDbContext _context;

    public LikeRepository(DaitingAppDbContext context)
    {
        _context = context;
    }



    public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, likedUserId);


    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var users =  _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        if (likesParams.Predicate== "liked")
        {
            likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
            users = likes.Select(like => like.LikedUser);
        }

        if (likesParams.Predicate == "likedBy")
        {
            likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
            users = likes.Select(like => like.SourceUser);
        }

        var likedUser= users.Select(user => new LikeDto
        {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
            City = user.City,
            Id = user.Id
        });

        return await PagedList<LikeDto>.CreateAsync(likedUser, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
       return await _context.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}
