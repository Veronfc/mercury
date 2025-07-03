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
      User user = await _userManager.GetUserAsync(User);

      return Ok(new UserInfoResDto(user.Email, user.DisplayName));
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<IActionResult> LogOut()
    {
      await _signInManager.SignOutAsync();
      return Ok("User logged out succesfully");
    }
  }
}
