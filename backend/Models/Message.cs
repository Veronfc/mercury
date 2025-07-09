using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
  public class Message
  {
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; }
    public string SenderId { get; set; }
    public User Sender { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
  }
}

namespace backend.Models
{
  public record MessageDto(Guid Id, string ConversationId, string SenderId, string Content, DateTime SentAt);
}