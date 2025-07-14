namespace backend.Models
{
  public class UserFriend
  {
    public string RequesterId { get; set; }
    public User Requester { get; set; }
    public string ReceiverId { get; set; }
    public User Receiver { get; set; }
    public DateTime RequestSentAt { get; set; }
    public DateTime? RequestAcceptedAt { get; set; }
    public UserFriendStatus Status { get; set; }
  }

  public enum UserFriendStatus
  {
    Pending,
    Accepted,
    Rejected
  }

  public record UserFriendDto(string FriendId, UserDto Friend, DateTime? FriendSince);
  public record UserFriendRequestSentDto(string ReceiverId, UserDto Receiver, DateTime SentAt, UserFriendStatus Status);
  public record UserFriendRequestReceivedDto(string RequesterId, UserDto Requester, DateTime SentAt, UserFriendStatus Status);
  
  public record RequestDto(string UserId);
}