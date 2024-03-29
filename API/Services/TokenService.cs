﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{
   private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));        

    }

   public string CreateToken(AppUser user)
    {
        try
        {
            var claims= new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId,user.UserName) //username
            };

            var creds= new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature); //signing key and algorithm to use

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject =new ClaimsIdentity(claims),
                Expires=DateTime.UtcNow.AddDays(7),
                SigningCredentials =  creds                
            };

            var tokenHandler=  new JwtSecurityTokenHandler ();
            var token=tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            //  log error
            Console.WriteLine(ex.Message);
            return null;
        }
    }

}



