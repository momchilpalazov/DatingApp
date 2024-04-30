
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController: BaseController
{

    
    private readonly  ITokenService _tokenService;

    private readonly IMapper _mapper;

    private readonly UserManager<AppUser> _userManager;


    public AccountController(UserManager<AppUser> userManager , ITokenService tokenService, IMapper mapper)
    {
        
        _tokenService = tokenService;
        _mapper = mapper;
        _userManager = userManager;

        
    }
   

    [HttpPost]
    [Route("register")] //api/account/register
    public async Task <ActionResult<UserDto>> Register(RegisterDto register)
    {

        if(await UserExists(register.Username)) return BadRequest("Username is taken");

        if (!register.Password.Any(char.IsDigit))
        {
            return BadRequest("Password must contain at least one digit");
        }

        var user = _mapper.Map<AppUser>(register);            

        
        
            user.UserName = register.Username.ToLower();
            
        

        var result = await _userManager.CreateAsync(user, register.Password);


        if (!result.Succeeded){
        foreach (var error in result.Errors)
        {
            // Отпечат грешките в конзолата или лог файла
            Console.WriteLine($"Error: {error.Code}, Description: {error.Description}");
        }
}

        if(!result.Succeeded) return BadRequest(result.Errors);

        var roleResult=await _userManager.AddToRoleAsync(user,"Member");

        if(!roleResult.Succeeded) return BadRequest(result.Errors);

        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
           
        };
       

    }


    [HttpPost]
    [Route("login")] //api/account/login
    public async Task <ActionResult<UserDto>> Login(LoginDto login)
    {

        var user = await _userManager.Users
        .Include(p=>p.Photos)
        .SingleOrDefaultAsync(u=>u.UserName==login.Username);

        if(user==null) return Unauthorized("Invalid username");

        var result = await _userManager.CheckPasswordAsync(user, login.Password);

        if(!result) return Unauthorized("Invalid password");

        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender

        };    
        

    }


    private async Task<bool> UserExists(string usrename)
    {
        return await _userManager.Users.AnyAsync(u=>u.UserName==usrename.ToLower());
    }
    


}