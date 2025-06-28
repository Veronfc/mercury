using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
  public class ConversationMember
  {
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public DateTime JoinedAt { get; set; }
    //public Guid? LastReadMessageId { get; set; }
    //public Message LastReadMessage { get; set; }
    public bool? IsAdmin { get; set; }
    public bool? IsMuted { get; set; }

  }
}