using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace backend.Hubs
{
  [Authorize]
  public class ConversationHub(DatabaseContext db, UserManager<User> userManager) : Hub
  {
    private readonly DatabaseContext _db = db;
    private readonly UserManager<User> _userManager = userManager;

    public override async Task OnConnectedAsync()
    {
      if (Context.UserIdentifier is not string userId)
      {
        return;
      }

      List<Guid> conversationIds = await _db.ConversationMembers.Where(cm => cm.UserId == userId).Select(cm => cm.ConversationId).ToListAsync();

      foreach (Guid id in conversationIds)
      {
        await Groups.AddToGroupAsync(Context.ConnectionId, id.ToString());
      }

      await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
      if (Context.UserIdentifier is not string userId)
      {
        return;
      }

      if (await _userManager.FindByIdAsync(userId) is not User user)
      {
        return;
      }

      user.LastActive = DateTime.UtcNow;
      await _userManager.UpdateAsync(user);

      await base.OnDisconnectedAsync(ex);
    }

    //TODO add a userId/connectionId mapping to automatically add other users to a new conversation group
    public async Task JoinConversation(Guid conversationId)
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    public async Task SendMessage(Guid conversationId, string content)
    {
        if (Context.UserIdentifier is not string userId)
        {
          return;
        }

        Message message = new()
        {
          Id = Guid.NewGuid(),
          ConversationId = conversationId,
          SenderId = userId,
          Content = content,
          SentAt = DateTime.UtcNow
        };

        if (!await _db.Conversations.AnyAsync(c => c.Id == conversationId))
        {
          throw new HubException("Invalid conversation ID");
        }

        Conversation conversation = new() { Id = conversationId };
        _db.Attach(conversation);
        _db.Entry(conversation).Property(c => c.LastMessageSentAt).CurrentValue = message.SentAt;
        _db.Entry(conversation).Property(c => c.LastMessageSentAt).IsModified = true;

        string snippet = message.Content[..Math.Min(100, message.Content.Length)];

        _db.Entry(conversation).Property(c => c.LastMessageSnippet).CurrentValue = snippet;
        _db.Entry(conversation).Property(c => c.LastMessageSnippet).IsModified = true;

        _db.Entry(conversation).Property(c => c.LastMessageSenderId).CurrentValue = userId;
        _db.Entry(conversation).Property(c => c.LastMessageSenderId).IsModified = true;

        await _db.Messages.AddAsync(message);
        await _db.SaveChangesAsync();

        MessageDto messageDto = new(message.Id, message.ConversationId.ToString(), message.SenderId, message.Content, message.SentAt);

        await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", messageDto);
    }
  }
}