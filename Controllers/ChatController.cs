using BookStoreApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ChatController : ControllerBase
{
    private readonly BookStoreDbContext _context;

    public ChatController(
        BookStoreDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetMessages(
        string userId)
    {
        var currentUserId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var messages =
            await _context.ChatMessages
                .Where(x =>
                    (x.SenderUserId == currentUserId &&
                     x.ReceiverUserId == userId)
                    ||
                    (x.SenderUserId == userId &&
                     x.ReceiverUserId == currentUserId))
                .OrderBy(x => x.SentAt)
                .ToListAsync();

        return Ok(messages);
    }

    [HttpGet("unread-counts")]
    public async Task<IActionResult> GetUnreadCounts()
    {
        var currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var unreadCounts =
            await _context.ChatMessages
                .Where(x =>
                    x.ReceiverUserId == currentUserId &&
                    !x.IsRead)
                .GroupBy(x => x.SenderUserId)
                .Select(g => new
                {
                    userId = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

        return Ok(unreadCounts);
    }
}