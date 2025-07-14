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

    [HttpPost("direct")]
    [Authorize]
    public async Task<ActionResult<ConversationDto>> NewDirectConversation([FromBody] NewDirectConversationDto body)
    {
      //TODO code to get other user by email/display name
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId)
      {
        return BadRequest("ID could not be determined");
      }

      if (body.UserId == userId)
      {
        return BadRequest("You can not start a conversation with yourself");
      }

      if (await _userManager.FindByIdAsync(body.UserId) is not User existingUser)
      {
        return NotFound($"User with ID: {body.UserId} does not exist");
      }

      if (await _db.Conversations.Where(c => c.Type == ConversationType.Direct).SingleOrDefaultAsync(c => c.Members.Count == 2 && c.Members.Any(m => m.UserId == body.UserId) && c.Members.Any(m => m.UserId == userId)) is Conversation existingConversation)
      {
        return Conflict($"Conversation with ID: {existingConversation.Id} already exists");
      }
      ;

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

      Conversation createdConversation = await _db.Conversations.Include(c => c.Members).ThenInclude(cm => cm.User).SingleAsync(c => c.Id == conversation.Id);

      ConversationDto conversationDto = new
      (
        createdConversation.Id,
        createdConversation.Type,
        createdConversation.Name,
        createdConversation.LastMessageSentAt,
        createdConversation.LastMessageSnippet,
        createdConversation.LastMessageSenderId,
        [.. createdConversation.Members.Select(m => new ConversationMemberDto
        (
          m.UserId,
          new UserDto
          (
            m.User.Id,
            m.User.Email,
            m.User.UserName,
            m.User.DisplayName,
            m.User.AvatarUrl,
            m.User.LastActive
          )
        ))]
      );

      return Created("", conversationDto);
    }

    [HttpPost("group")]
    [Authorize]
    public async Task<ActionResult<ConversationDto>> NewGroupConversation([FromBody] NewGroupConversationDto body)
    {
      //TODO code to get other user by email/display name
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId)
      {
        return BadRequest("ID could not be determined");
      }

      if (body.UserIds.Contains(userId))
      {
        return BadRequest("You can not include yourself in a group conversation twice");
      }

      foreach (string id in body.UserIds)
      {
        if (await _userManager.FindByIdAsync(id) is not User existingUser)
        {
          return NotFound($"User with ID: {id} does not exist");
        }
      }

      Conversation conversation = new()
      {
        Id = Guid.NewGuid(),
        Type = ConversationType.Group,
        Name = body.Name,
        CreatedAt = DateTime.UtcNow,
        CreatorId = userId,
        Members = [.. body.UserIds.Select(id => new ConversationMember()
        {
          UserId = id,
          JoinedAt = DateTime.UtcNow
        })]
      };

      conversation.Members.Add(new()
      {
        UserId = userId,
        JoinedAt = DateTime.UtcNow
      });

      await _db.Conversations.AddAsync(conversation);
      await _db.SaveChangesAsync();

      Conversation createdConversation = await _db.Conversations.Include(c => c.Members).ThenInclude(cm => cm.User).SingleAsync(c => c.Id == conversation.Id);

      ConversationDto conversationDto = new
      (
        createdConversation.Id,
        createdConversation.Type,
        createdConversation.Name,
        createdConversation.LastMessageSentAt,
        createdConversation.LastMessageSnippet,
        createdConversation.LastMessageSenderId,
        [.. createdConversation.Members.Select(m => new ConversationMemberDto
        (
          m.UserId,
          new UserDto
          (
            m.User.Id,
            m.User.Email,
            m.User.UserName,
            m.User.DisplayName,
            m.User.AvatarUrl,
            m.User.LastActive
          )
        ))]
      );

      return Created("", conversationDto);
    }

    [HttpGet("all")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConversations()
    {
      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId)
      {
        return BadRequest("ID could not be determined");
      }

      List<Conversation> conversations = await _db.Conversations
        .Where(c => c.Members.Any(cm => cm.UserId == userId))
        .Include(c => c.Members)
        .ThenInclude(cm => cm.User)
        .OrderByDescending(c => c.LastMessageSentAt)
        .ToListAsync();

      List<ConversationDto> conversationsDto = [.. conversations.Select(c => new ConversationDto
      (
        c.Id,
        c.Type,
        c.Name,
        c.LastMessageSentAt,
        c.LastMessageSnippet,
        c.LastMessageSenderId,
        [.. c.Members.Select(m => new ConversationMemberDto
        (
          m.UserId,
          new UserDto
          (
            m.User.Id,
            m.User.Email,
            m.User.UserName,
            m.User.DisplayName,
            m.User.AvatarUrl,
            m.User.LastActive
          )
        ))]
      ))];

      return Ok(conversationsDto);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Conversation>> GetConversation(string id)
    {
      if (!Guid.TryParse(id, out Guid guid))
      {
        return BadRequest();
      }

      if (await _db.Conversations.FindAsync(guid) is not Conversation conversation)
      {
        return NotFound();
      }

      return conversation;
    }
  }
}