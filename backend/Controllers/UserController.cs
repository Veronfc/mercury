using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
      _userManager = userManager;
      _signInManager = signInManager;
    }

    [HttpGet]
    [Authorize]
    public IActionResult LoggedIn()
    {
      return Ok();
    }

    [HttpGet("info")]
    [Authorize]
    public async Task<ActionResult<UserInfoResDto>> Info()
    {
      User user = await _userManager.FindByNameAsync(User.Identity.Name);

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
