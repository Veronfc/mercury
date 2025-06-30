using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
  [ApiController]
  [Route("messages")]
  public class MessageController(DatabaseContext db) : ControllerBase
  {
    private readonly DatabaseContext _db = db;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessages([FromQuery] string conversationId)
    {
      if (!Guid.TryParse(conversationId, out Guid id))
      {
        return BadRequest("Conversation ID is not valid");
      }

      List<Message> messages = await _db.Messages.Where(m => m.ConversationId == id).OrderByDescending(m => m.SentAt).ToListAsync();

      if (messages.Count == 0)
      {
        return NotFound();
      }

      return Ok(messages);
    }
  }
}