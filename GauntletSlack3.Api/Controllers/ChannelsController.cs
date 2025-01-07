using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Api.Data;
using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChannelsController : ControllerBase
{
    private readonly SlackDbContext _context;

    public ChannelsController(SlackDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Channel>>> GetChannels([FromQuery] string userId)
    {
        return await _context.Channels
            .Include(c => c.ChannelUsers)
            .Where(c => c.Type == "public" || 
                       c.ChannelUsers.Any(cu => cu.UserId == userId))
            .ToListAsync();
    }

    [HttpGet("{channelId}/messages")]
    public async Task<ActionResult<IEnumerable<Message>>> GetChannelMessages(int channelId)
    {
        return await _context.Messages
            .Where(m => m.ChannelId == channelId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    [HttpPost("{channelId}/join")]
    public async Task<IActionResult> JoinChannel(int channelId, ChannelUser channelUser)
    {
        if (channelId != channelUser.ChannelId)
        {
            return BadRequest();
        }

        _context.ChannelUsers.Add(channelUser);
        await _context.SaveChangesAsync();

        return Ok();
    }
} 