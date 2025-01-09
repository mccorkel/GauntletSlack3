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

        [HttpGet]
        public async Task<ActionResult<List<Channel>>> GetChannels([FromQuery] int userId)
        {
            var channels = await _context.Channels
                .Include(c => c.Memberships)
                .Include(c => c.Messages)
                .ThenInclude(m => m.User)
                .Where(c => c.Memberships.Any(m => m.UserId == userId) || 
                           c.Type != "private")
                .ToListAsync();

            Console.WriteLine($"API: Getting all channels, found {channels.Count}");
            Console.WriteLine($"API: Filtering for user {userId}");
            foreach (var channel in channels)
            {
                var isMember = channel.Memberships.Any(m => m.UserId == userId);
                Console.WriteLine($"API: Channel {channel.Id}: {channel.Name} - Type: {channel.Type} - IsMember: {isMember}");
            }

            return channels;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Channel>>> GetUserChannels(int userId)
        {
            Console.WriteLine($"API: Getting channels for user {userId}");
            
            // Check if user exists
            var user = await _context.Users.FindAsync(userId);
            Console.WriteLine($"API: User exists: {user != null}");

            // Check all channels
            var allChannels = await _context.Channels.ToListAsync();
            Console.WriteLine($"API: Total channels in database: {allChannels.Count}");

            // Check memberships
            var memberships = await _context.ChannelMemberships
                .Where(m => m.UserId == userId)
                .ToListAsync();
            Console.WriteLine($"API: User memberships found: {memberships.Count}");

            var channels = await _context.Channels
                .Include(c => c.Memberships)
                .Include(c => c.Messages)
                .ThenInclude(m => m.User)
                .Where(c => c.Memberships.Any(m => m.UserId == userId))
                .ToListAsync();
            
            Console.WriteLine($"API: Found {channels.Count} channels for user {userId}");
            foreach (var channel in channels)
            {
                Console.WriteLine($"API: Channel {channel.Id}: {channel.Name} with {channel.Memberships?.Count ?? 0} members");
            }
            
            return channels;
        }

        [HttpPost]
        public async Task<ActionResult<Channel>> CreateChannel([FromBody] CreateChannelRequest request)
        {
            var channel = new Channel
            {
                Name = request.Name,
                Type = request.Type,
                CreatedAt = DateTime.UtcNow,
                OwnerId = request.UserId
            };

            _context.Channels.Add(channel);
            await _context.SaveChangesAsync();

            _context.ChannelMemberships.Add(new ChannelMembership
            {
                ChannelId = channel.Id,
                UserId = request.UserId,
                JoinedAt = DateTime.UtcNow,
                IsMuted = false
            });

            await _context.SaveChangesAsync();

            return await _context.Channels
                .Include(c => c.Memberships)
                .Include(c => c.Messages)
                .ThenInclude(m => m.User)
                .FirstAsync(c => c.Id == channel.Id);
        }

        [HttpGet("{channelId}/members")]
        public async Task<ActionResult<List<User>>> GetChannelMembers(int channelId)
        {
            return await _context.Users
                .Where(u => u.Memberships.Any(m => m.ChannelId == channelId))
                .ToListAsync();
        }

        [HttpPost("{channelId}/join")]
        public async Task<ActionResult> JoinChannel(int channelId, [FromBody] int userId)
        {
            var membership = await _context.ChannelMemberships
                .FirstOrDefaultAsync(m => m.ChannelId == channelId && m.UserId == userId);

            if (membership != null)
            {
                return BadRequest("User is already a member of this channel");
            }

            _context.ChannelMemberships.Add(new ChannelMembership
            {
                ChannelId = channelId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow,
                IsMuted = false
            });

            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class CreateChannelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
} 