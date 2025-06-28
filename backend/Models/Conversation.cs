using System.ComponentModel.DataAnnotations.Schema;

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
    public ICollection<ConversationMember> Members { get; } = [];
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