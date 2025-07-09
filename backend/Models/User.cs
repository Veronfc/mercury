using Microsoft.AspNetCore.Identity;

namespace backend.Models
{
  public class User : IdentityUser
  {
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime LastActive { get; set; }
    public ICollection<ConversationMember> Conversations { get; } = [];
  }
}

namespace backend.Models
{
  public record UserInfoResDto(string Email, string DisplayName, string Id);
}

namespace backend.Models
{
  public record UserDto(string Id, string Email, string UserName, string DisplayName);
}