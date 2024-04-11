using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController: BaseController
{

    private readonly DaitingAppDbContext _context;

    private readonly  ITokenService _tokenService;

    private readonly IMapper _mapper;


    public AccountController(DaitingAppDbContext context, ITokenService tokenService, IMapper mapper)
    {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
        
    }
   

    [HttpPost]
    [Route("register")] //api/account/register
    public async Task <ActionResult<UserDto>> Register(RegisterDto register)
    {

        if(await UserExists(register.Username)) return BadRequest("Username is taken");

        var user = _mapper.Map<AppUser>(register);

        using var hmac = new HMACSHA512();        

        
        
            user.UserName = register.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
            user.PasswordSalt = hmac.Key;
        

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            KnownAs = user.KnownAs
           
        };
       

    }


    [HttpPost]
    [Route("login")] //api/account/login
    public async Task <ActionResult<UserDto>> Login(LoginDto login)
    {

        var user = await _context.Users
        .Include(p=>p.Photos)
        .SingleOrDefaultAsync(u=>u.UserName==login.Username);

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
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
            KnownAs = user.KnownAs

        };    
        

    }


    private async Task<bool> UserExists(string usrename)
    {
        return await _context.Users.AnyAsync(u=>u.UserName==usrename.ToLower());
    }
    


}