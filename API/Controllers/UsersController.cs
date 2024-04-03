using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseController
{
    
   private readonly IUserRepository _userRepository;

   private readonly IMapper _mapper;

   public UsersController(IUserRepository userRepository,IMapper mapper)
   {
    _userRepository=userRepository;
    _mapper=mapper;

    
   }
    
    [HttpGet] //api/users    
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _userRepository.GetMembersDtosAsync();     

        return Ok(users);

    }

    
    [HttpGet("{username}")]   //api/users/id
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var users = await _userRepository.GetMemberDtoAsync(username);

        return users;       
        
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _userRepository.GetUserByUsernameAsync(username);

        _mapper.Map(memberUpdateDto,user);

        _userRepository.Update(user);

        if(await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");
    }





}
