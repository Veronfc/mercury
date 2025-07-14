using System.Net.Sockets;
using System.Security.Claims;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace backend.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UserController(SignInManager<User> signInManager, UserManager<User> userManager, DatabaseContext db) : ControllerBase
  {
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly DatabaseContext _db = db;

    [HttpGet()]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetInfo()
    {
      if (await _userManager.GetUserAsync(User) is not User user)
      {
        return BadRequest("User profile not found");
      }

      return Ok(new UserDto
      (
        user.Id,
        user.Email,
        user.UserName,
        user.DisplayName,
        user.AvatarUrl,
        user.LastActive
      ));
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<IActionResult> LogOut()
    {
      await _signInManager.SignOutAsync();

      return Ok("User logged out succesfully");
    }

    [HttpPost("displayname")]
    [Authorize]
    public async Task<IActionResult> SetDisplayName([FromBody] SetDisplayNameDto body)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (await _userManager.GetUserAsync(User) is not User user)
      {
        return BadRequest("User profile not found");
      }

      user.DisplayName = body.DisplayName;

      try
      {
        await _userManager.UpdateAsync(user);
      }
      catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
      {
        return Conflict("Display name is already in use");
      }

      return Ok("Display name set");
    }
  }
}
