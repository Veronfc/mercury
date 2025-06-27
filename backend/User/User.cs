using Microsoft.AspNetCore.Identity;

namespace backend.User
{
  public class User : IdentityUser
  {
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTimeOffset LastActive { get; set; } = DateTimeOffset.UtcNow;
  }
}

namespace backend.User
{
  public record UserInfoResDto(string Email, string DisplayName);
}