using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace backend.Hubs
{
  [Authorize]
  public class ConversationHub(DatabaseContext db) : Hub
  {
    private readonly DatabaseContext _db = db;

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

      await _db.Messages.AddAsync(message);
      await _db.SaveChangesAsync();

      await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", message);
    }
  }
}