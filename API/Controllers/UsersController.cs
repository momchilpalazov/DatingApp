using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseController
{
    
   private readonly IUserRepository _userRepository;

   public UsersController(IUserRepository userRepository)
   {
    _userRepository=userRepository;
    
   }
    
    [HttpGet] //api/users    
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await _userRepository.GetUsersAsync();
         return  users.ToList();

    }

    
    [HttpGet("{username}")]   //api/users/id
    public async Task<ActionResult<AppUser>> GetUser(string username)
    {
        var users = await _userRepository.GetUserByUsernameAsync(username);
        return users;
    }





}
