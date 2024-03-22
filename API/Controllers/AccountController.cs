using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController: BaseController
{

    private readonly DaitingAppDbContext _context;

    private readonly  ITokenService _tokenService;


    public AccountController(DaitingAppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
        
    }
   

    [HttpPost]
    [Route("register")] //api/account/register
    public async Task <ActionResult<UserDto>> Register(RegisterDto register)
    {

        if(await UserExists(register.Username)) return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();        

        var user = new AppUser
        {
            UserName = register.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
       

    }


    [HttpPost]
    [Route("login")] //api/account/login
    public async Task <ActionResult<UserDto>> Login(LoginDto login)
    {

        var user = await _context.Users.SingleOrDefaultAsync(u=>u.UserName==login.Username);

        if(user==null) return Unauthorized("Invalid username");

        using var nmac =new HMACSHA512(user.PasswordSalt);

        var computedHash=nmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
           if (computedHash[i]!=user.PasswordHash[i]) return Unauthorized("Invalid password" );
        }

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };    
        

    }


    private async Task<bool> UserExists(string usrename)
    {
        return await _context.Users.AnyAsync(u=>u.UserName==usrename.ToLower());
    }
    


}