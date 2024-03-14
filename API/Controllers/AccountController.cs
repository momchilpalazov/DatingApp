using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController: BaseController
{

    private readonly DaitingAppDbContext _context;


    public AccountController(DaitingAppDbContext context)
    {
        _context = context;
    }


    [HttpPost]
    [Route("register")] //api/account/register
    public async Task <ActionResult<AppUser>> Register(RegisterDto register)
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
        return user;

    }

    private async Task<bool> UserExists(string usrename)
    {
        return await _context.Users.AnyAsync(u=>u.UserName==usrename.ToLower());
    }
    


}