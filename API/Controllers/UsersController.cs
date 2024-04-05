﻿using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[Authorize]
public class UsersController : BaseController
{
    
   private readonly IUserRepository _userRepository;

   private readonly IMapper _mapper;

   private readonly IPhotoService _photoService;

   public UsersController(IUserRepository userRepository,IMapper mapper,IPhotoService photoService)
   {
    _userRepository=userRepository;
    _mapper=mapper;
    _photoService=photoService;

    
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
        var username = User.GetUserName();

        var user = await _userRepository.GetUserByUsernameAsync(username);

        if(user == null) return NotFound();

        _mapper.Map(memberUpdateDto,user);

        _userRepository.Update(user);

        if(await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        // var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var username = User.GetUserName();

        var user = await _userRepository.GetUserByUsernameAsync(username);

        if(user == null) return NotFound();

        var result = await _photoService.AddPhotoAsync(file);

        if(result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if(user.Photos.Count == 0)
        {
            photo.IsMain = true;
        }

        user.Photos.Add(photo);

        if(await _userRepository.SaveAllAsync())
        {
            return CreatedAtAction(nameof(GetUser),
            new {username = user.UserName},_mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("Problem adding photo");
    }





}
