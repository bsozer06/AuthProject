using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    // Accessible by anyone with a valid token
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        return Ok(new { 
            Message = "You are authenticated!", 
            User = User.Identity?.Name,
            Claims = User.Claims.Select(c => new { c.Type, c.Value }) 
        });
    }

    // Accessible only by users with "Permissions.Users.Read" (Admin & Manager)
    [Authorize(Policy = "CanReadUsers")]
    [HttpGet("list")]
    public IActionResult GetUsers()
    {
        return Ok("This is a protected list of users.");
    }

    // Accessible only by users with "Permissions.Users.Delete" (Admin only)
    [Authorize(Policy = "CanDeleteUsers")]
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        return Ok($"User {id} has been deleted.");
    }
}