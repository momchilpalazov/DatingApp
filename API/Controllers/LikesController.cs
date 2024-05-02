using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController: BaseController
{

    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public LikesController(IUnitOfWorkRepository unitOfWorkRepository)
    {
        _unitOfWorkRepository = unitOfWorkRepository;
    }
   


    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId =int.Parse( User.GetUserId());
        var likedUser = await _unitOfWorkRepository.UserRepository.GetUserByUsernameAsync(username);
        var likedUserId = likedUser.Id;
        var sourceUser = await _unitOfWorkRepository.LikeRepository.GetUserWithLikes(sourceUserId);

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _unitOfWorkRepository.LikeRepository.GetUserLike(sourceUserId, likedUserId);

        if (userLike != null) return BadRequest("You already liked this user");

        userLike = new UserLike        
        {
            SourceUserId = sourceUserId,
            LikedUserId = likedUserId
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _unitOfWorkRepository.Complete()) return Ok();

        return BadRequest("Failed to like user");
    }
    
    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
    {

        likesParams.UserId = int.Parse(User.GetUserId());        
        var users = await _unitOfWorkRepository.LikeRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPage);
        return Ok(users);
    }
}
