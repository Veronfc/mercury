using System.Security.Claims;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
  [ApiController]
  [Route("conversations")]
  public class ConversationController(DatabaseContext db) : ControllerBase
  {
    private readonly DatabaseContext _db = db;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Conversation>> CreateConversation([FromBody] CreateDirectConversationDto body)
    {
      //ADD: code to get other user by email
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      Conversation existingConversation = await _db.Conversations.Where(c => c.Type == ConversationType.Direct).Include(c => c.Members).SingleOrDefaultAsync(c => c.Members.Count == 2 && c.Members.Any(m => m.UserId == body.UserId) && c.Members.Any(m => m.UserId == userId));

      if (existingConversation != null)
      {
        return Ok(existingConversation);
      }

      Conversation conversation = new()
      {
        Id = Guid.NewGuid(),
        Type = ConversationType.Direct,
        CreatedAt = DateTime.UtcNow,
        CreatorId = userId,
        Members =
        [
          new() {UserId = userId, JoinedAt = DateTime.UtcNow},
          new() {UserId = body.UserId, JoinedAt = DateTime.UtcNow}
        ]
      };

      await _db.Conversations.AddAsync(conversation);
      await _db.SaveChangesAsync();

      return Created("", conversation);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Conversation>>> GetConversations()
    {
      string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      List<Conversation> conversations = await _db.ConversationMembers
        .Where(cm => cm.UserId == userId)
        .Include(cm => cm.Conversation)
        .ThenInclude(c => c.Members)
        .ThenInclude(cm => cm.User)
        .Select(cm => cm.Conversation)
        .OrderByDescending(c => c.LastMessageSentAt)
        .ToListAsync();

      return Ok(conversations);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Conversation>> GetConversation(string id)
    {
      if (!Guid.TryParse(id, out Guid guid))
      {
        return BadRequest("");
      }

      Conversation conversation = await _db.Conversations.FindAsync(guid);

      if (conversation == null)
      {
        return NotFound();
      }

      return conversation;
    }
  }
}