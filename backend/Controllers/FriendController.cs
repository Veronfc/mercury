using System.Security.Claims;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
  [ApiController]
  [Route("friends")]
  public class FriendController(UserManager<User> userManager, DatabaseContext db) : ControllerBase
  {
    private readonly UserManager<User> _userManager = userManager;
    private readonly DatabaseContext _db = db;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserFriendDto>>> GetFriends()
    {
      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId)
      {
        return BadRequest("User profile not found");
      }

      List<UserFriend> friends = await _db.UserFriends.Where(uf => (uf.ReceiverId == userId || uf.RequesterId == userId) && uf.Status == UserFriendStatus.Accepted).Include(uf => uf.Requester).Include(uf => uf.Receiver).ToListAsync();

      List<UserFriendDto> friendsDto = [.. friends.Select(uf =>
      {
        User friend = uf.RequesterId == userId ? uf.Requester : uf.Receiver;

        return new UserFriendDto(
          friend.Id,
          new UserDto(
            friend.Id,
            friend.Email,
            friend.UserName,
            friend.DisplayName,
            friend.AvatarUrl,
            friend.LastActive
          ),
          uf.RequestAcceptedAt
        );
      })];

      return Ok(friendsDto);
    }

    [HttpGet("received")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserFriendRequestReceivedDto>>> GetRequestsReceived()
    {
      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId)
      {
        return BadRequest("User profile not found");
      }

      List<UserFriend> requestsReceived = await _db.UserFriends.Where(uf => uf.ReceiverId == userId).Include(uf => uf.Requester).ToListAsync();

      List<UserFriendRequestReceivedDto> requestsReceivedDto = [.. requestsReceived.Select(uf => new UserFriendRequestReceivedDto(
        uf.Requester.Id,
        new UserDto(
          uf.Requester.Id,
          uf.Requester.Email,
          uf.Requester.UserName,
          uf.Requester.DisplayName,
          uf.Requester.AvatarUrl,
          uf.Requester.LastActive
        ),
        uf.RequestSentAt,
        uf.Status
      ))];

      return Ok(requestsReceivedDto);
    }

    [HttpGet("sent")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserFriendRequestSentDto>>> GetRequestsSent()
    {
      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId)
      {
        return BadRequest("User profile not found");
      }

      List<UserFriend> requestsSent = await _db.UserFriends.Where(uf => uf.RequesterId == userId).Include(uf => uf.Receiver).ToListAsync();

      List<UserFriendRequestSentDto> requestsSentDto = [.. requestsSent.Select(uf => new UserFriendRequestSentDto(
        uf.Receiver.Id,
        new UserDto(
          uf.Receiver.Id,
          uf.Receiver.Email,
          uf.Receiver.UserName,
          uf.Receiver.DisplayName,
          uf.Receiver.AvatarUrl,
          uf.Receiver.LastActive
        ),
        uf.RequestSentAt,
        uf.Status
      ))];

      return Ok(requestsSentDto);
    }

    [HttpPost("request")]
    [Authorize]
    public async Task<IActionResult> NewRequest([FromBody] RequestDto body)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId)
      {
        return BadRequest("ID could not be determined");
      }

      if (userId == body.UserId)
      {
        return BadRequest("You can not send a friend request to yourself");
      }

      if (await _userManager.FindByIdAsync(body.UserId) is not User existingUser)
      {
        return NotFound($"User with ID: {body.UserId} does not exist");
      }

      if (await _db.UserFriends.SingleOrDefaultAsync(uf => uf.RequesterId == body.UserId || uf.ReceiverId == body.UserId) is UserFriend existingFriend)
      {
        if (existingFriend.Status == UserFriendStatus.Accepted)
        {
          return Conflict($"You are already friends with {existingUser.DisplayName ?? existingUser.UserName}");
        }

        return Conflict($"There is already a pending friend request with {existingUser.DisplayName ?? existingUser.UserName}");
      }

      UserFriend friend = new()
      {
        Requester = new()
        {
          Id = userId
        },
        Receiver = new()
        {
          Id = body.UserId
        },
        RequestSentAt = DateTime.UtcNow,
        Status = UserFriendStatus.Pending
      };

      await _db.UserFriends.AddAsync(friend);
      await _db.SaveChangesAsync();

      return Created("", $"Request successfully sent to {existingUser.DisplayName ?? existingUser.UserName}");
    }

    [HttpPost("accept")]
    [Authorize]
    public async Task<IActionResult> AcceptRequest([FromBody] RequestDto body)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId)
      {
        return BadRequest("ID could not be determined");
      }

      if (await _userManager.FindByIdAsync(body.UserId) is not User existingUser)
      {
        return NotFound($"User with ID: {body.UserId} does not exist");
      }

      if (await _db.UserFriends.FindAsync(body.UserId, userId) is not UserFriend existingRequest)
      {
        return NotFound("Friend request not found");
      }

      existingRequest.Status = UserFriendStatus.Accepted;
      existingRequest.RequestAcceptedAt = DateTime.UtcNow;
      await _db.SaveChangesAsync();

      return Ok("Friend request accepted");
    }

    [HttpPost("reject")]
    [Authorize]
    public async Task<IActionResult> RejectRequest([FromBody] RequestDto body)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not string userId)
      {
        return BadRequest("ID could not be determined");
      }

      if (await _userManager.FindByIdAsync(body.UserId) is not User existingUser)
      {
        return NotFound($"User with ID: {body.UserId} does not exist");
      }

      if (await _db.UserFriends.FindAsync(body.UserId, userId) is not UserFriend existingRequest)
      {
        return NotFound("Friend request not found");
      }

      existingRequest.Status = UserFriendStatus.Rejected;
      await _db.SaveChangesAsync();

      return Ok("Friend request rejected");
    }
  }
}