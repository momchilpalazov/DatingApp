
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController: BaseController
{

    private readonly UserManager<AppUser> _userManager;

    private readonly DaitingAppDbContext _context;

    public AdminController(UserManager<AppUser> userManager, DaitingAppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
  

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUserWithRolesAsync()
    {
        var users = await _userManager.Users
        .OrderBy(x => x.UserName)
        .Select(user => new
        {
            user.Id,
            Username = user.UserName,
            Roles = (from userRole in user.UserRoles
                join role in _context.Roles
                on userRole.RoleId
                equals role.Id
                select role.Name).ToList()
          
        }).ToListAsync();

        return Ok(users);
        
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {
        if (roles == null) return BadRequest("You need to provide roles");
        var selectedRoles = roles.Split(",").ToArray();
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return NotFound("Could not find user");
        var userRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed to add to roles");
        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove from roles");
        return Ok(await _userManager.GetRolesAsync(user));
    }



    [Authorize(Policy = "ModeratePhotoRole")] 
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admins or Moderators can see this");
    }  
    
}
