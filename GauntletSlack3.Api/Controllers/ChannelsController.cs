using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Shared.Models;
using GauntletSlack3.Api.Data;

namespace GauntletSlack3.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelsController : ControllerBase
    {
        private readonly SlackDbContext _context;

        public ChannelsController(SlackDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Channel>> CreateChannel([FromBody] Channel channel)
        {
            channel.CreatedAt = DateTime.UtcNow;
            _context.Channels.Add(channel);
            await _context.SaveChangesAsync();
            return Ok(channel);
        }

        [HttpGet("{channelId}")]
        public async Task<ActionResult<Channel>> GetChannel(int channelId)
        {
            var channel = await _context.Channels
                .Include(c => c.Owner)
                .Include(c => c.Memberships!)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(c => c.Id == channelId);

            if (channel == null) return NotFound();
            return Ok(channel);
        }

        [HttpGet]
        public async Task<ActionResult<List<Channel>>> GetChannels()
        {
            var channels = await _context.Channels
                .Include(c => c.Owner)
                .Include(c => c.Memberships!)
                .ThenInclude(m => m.User)
                .ToListAsync();

            return Ok(channels);
        }

        [HttpPut("{channelId}")]
        public async Task<ActionResult<Channel>> UpdateChannel(int channelId, [FromBody] Channel channel)
        {
            if (channelId != channel.Id) return BadRequest();
            
            _context.Entry(channel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(channel);
        }

        [HttpDelete("{channelId}")]
        public async Task<ActionResult> DeleteChannel(int channelId)
        {
            var channel = await _context.Channels.FindAsync(channelId);
            if (channel == null) return NotFound();
            
            _context.Channels.Remove(channel);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{channelId}/users")]
        public async Task<ActionResult> AddUserToChannel(int channelId, [FromBody] string userId)
        {
            var membership = new ChannelMembership
            {
                ChannelId = channelId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            _context.ChannelMemberships.Add(membership);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{channelId}/users/{userId}")]
        public async Task<ActionResult> RemoveUserFromChannel(int channelId, string userId)
        {
            var membership = await _context.ChannelMemberships
                .FirstOrDefaultAsync(m => m.ChannelId == channelId && m.UserId == userId);
            
            if (membership == null) return NotFound();
            
            _context.ChannelMemberships.Remove(membership);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
} 