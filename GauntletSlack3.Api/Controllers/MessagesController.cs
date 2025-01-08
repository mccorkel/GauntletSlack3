using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Shared.Models;
using GauntletSlack3.Api.Data;

namespace GauntletSlack3.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly SlackDbContext _context;

        public MessagesController(SlackDbContext context)
        {
            _context = context;
        }

        // POST: api/messages/channel/{channelId}
        [HttpPost("channel/{channelId}")]
        public async Task<ActionResult<Message>> PostMessage(int channelId, [FromBody] Message message)
        {
            var channel = await _context.Channels
                .Include(c => c.Memberships)
                .FirstOrDefaultAsync(c => c.Id == channelId);

            if (channel == null)
            {
                return NotFound($"Channel with ID {channelId} not found");
            }

            // Check if user is a member of the channel
            var isMember = await _context.ChannelMemberships
                .AnyAsync(cm => cm.ChannelId == channelId && cm.UserId == message.UserId);

            if (!isMember)
            {
                return BadRequest("User is not a member of this channel");
            }

            message.ChannelId = channelId;
            message.CreatedAt = DateTime.UtcNow;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            
            return Ok(message);
        }

        // GET: api/messages/channel/{channelId}
        [HttpGet("channel/{channelId}")]
        public async Task<ActionResult<List<Message>>> GetChannelMessages(int channelId)
        {
            var messages = await _context.Messages
                .Where(m => m.ChannelId == channelId)
                .Include(m => m.User)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }

        // GET: api/messages/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await _context.Messages
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // DELETE: api/messages/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
} 