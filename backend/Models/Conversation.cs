using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
  public class Conversation
  {
    public Guid Id { get; set; }
    public ConversationType Type { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatorId { get; set; }
    public User Creator { get; set; }
    public DateTime? LastMessageSentAt { get; set; }
    public string? LastMessageSnippet { get; set; }
    public ICollection<ConversationMember> Members { get; set; } = [];
    public ICollection<Message> Messages { get; } = [];
  }
}

namespace backend.Models
{
  public enum ConversationType
  {
    Direct,
    Group
  }
}

namespace backend.Models
{
  public record class CreateDirectConversationDto
  {
    [Required]
    public string UserId { get; init; }

    //[EmailAddress]
    //public string? UserEmail { get; init; }
  }
}

namespace backend.Models
{
  public record ConversationDto(Guid Id, ConversationType Type, string? Name, DateTime? LastMessageSentAt, string? LastMessageSnippet, List<ConversationMemberDto> Members);
}