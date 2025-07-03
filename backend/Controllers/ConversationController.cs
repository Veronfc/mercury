using System.Security.Claims;
using backend.Hubs;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
  [ApiController]
  [Route("conversations")]
  public class ConversationController(DatabaseContext db, UserManager<User> userManager, IHubContext<ConversationHub> hubContext) : ControllerBase
  {
    private readonly DatabaseContext _db = db;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IHubContext<ConversationHub> _hubContext = hubContext;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Conversation>> CreateDirectConversation([FromBody] CreateDirectConversationDto body)
    {
      //TODO code to get other user by email
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (await _userManager.FindByIdAsync(body.UserId) is not User existingUser) {
        return NotFound($"User with ID: {body.UserId} does not exist");
      }

      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId) {
        return BadRequest("ID could not be determined");
      }

      if (await _db.Conversations.Where(c => c.Type == ConversationType.Direct).Include(c => c.Members).SingleOrDefaultAsync(c => c.Members.Count == 2 && c.Members.Any(m => m.UserId == body.UserId) && c.Members.Any(m => m.UserId == userId)) is Conversation existingConversation)
      {
        return Conflict($"Conversation with ID: {existingConversation.Id} already exists");
      };

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
      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId) {
        return BadRequest("ID could not be determined");
      }

      List<Conversation> conversations = await _db.Conversations
        .Where(c => c.Members.Any(cm => cm.UserId == userId))
        .Include(c => c.Members)
        .ThenInclude(cm => cm.User)
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

      if (await _db.Conversations.FindAsync(guid) is not Conversation conversation) {
        return NotFound();
      }

      return conversation;
    }
  }
}