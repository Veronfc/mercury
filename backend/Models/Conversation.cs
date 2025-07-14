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
    public string? LastMessageSenderId { get; set; }
    public ICollection<ConversationMember> Members { get; set; } = [];
    public ICollection<Message> Messages { get; } = [];
  }

  public enum ConversationType
  {
    Direct,
    Group
  }

  public record ConversationDto(Guid Id, ConversationType Type, string? Name, DateTime? LastMessageSentAt, string? LastMessageSnippet, string? LastMessageSenderId, List<ConversationMemberDto> Members);
  public record NewDirectConversationDto(string UserId);
  public record NewGroupConversationDto(string Name, List<string> UserIds);
}