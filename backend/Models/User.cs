using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
  [Index(nameof(DisplayName), IsUnique = true)]
  public class User : IdentityUser
  {
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime LastActive { get; set; }
    public ICollection<ConversationMember> Conversations { get; } = [];
    public ICollection<UserFriend> RequestsSent { get; } = [];
    public ICollection<UserFriend> RequestsReceieved { get; } = [];
  }

  public record UserDto(string Id, string Email, string UserName, string DisplayName, string AvatarUrl, DateTime LastActive);

  public record SetDisplayNameDto(string DisplayName);
}