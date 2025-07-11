using System.Security.Claims;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UserController(SignInManager<User> signInManager, UserManager<User> userManager) : ControllerBase
  {
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;

    [HttpGet("info")]
    [Authorize]
    public async Task<ActionResult<UserInfoResDto>> Info()
    {
      if (await _userManager.GetUserAsync(User) is not User user)
      {
        return BadRequest("User profile not found");
      }

      return Ok(new UserInfoResDto(user.Email, user.DisplayName, user.Id));
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

      await _userManager.UpdateAsync(user);

      return Ok("Display name set");
    }
  }
}
