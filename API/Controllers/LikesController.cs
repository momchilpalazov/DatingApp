using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController: BaseController
{

    private readonly IlikeRepository _likeRepository;

    private readonly IUserRepository _userRepository;

    public LikesController(IlikeRepository likeRepository, IUserRepository userRepository)
    {
        _likeRepository = likeRepository;
        _userRepository = userRepository;
    }


    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId =int.Parse( User.GetUserId());
        var likedUser = await _userRepository.GetUserByUsernameAsync(username);
        var likedUserId = likedUser.Id;
        var sourceUser = await _likeRepository.GetUserWithLikes(sourceUserId);

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _likeRepository.GetUserLike(sourceUserId, likedUserId);

        if (userLike != null) return BadRequest("You already liked this user");

        userLike = new UserLike        
        {
            SourceUserId = sourceUserId,
            LikedUserId = likedUserId
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Failed to like user");
    }
    
    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
    {

        likesParams.UserId = int.Parse(User.GetUserId());        
        var users = await _likeRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPage);
        return Ok(users);
    }
}
