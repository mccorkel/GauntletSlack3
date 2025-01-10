using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Shared.Models;
using GauntletSlack3.Api.Data;
using Microsoft.Extensions.Logging;

namespace GauntletSlack3.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelsController : ControllerBase
    {
        private readonly SlackDbContext _context;
        private readonly ILogger<ChannelsController> _logger;

        public ChannelsController(SlackDbContext context, ILogger<ChannelsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Channel>>> GetChannels([FromQuery] int userId)
        {
            try
            {
                _logger.LogInformation("Getting channels for user {UserId}", userId);

                var query = _context.Channels
                    .Include(c => c.Memberships)
                    .Include(c => c.Messages)
                    .ThenInclude(m => m.User)
                    .Where(c => c.Memberships.Any(m => m.UserId == userId));

                var channels = await query.ToListAsync();
                _logger.LogInformation("Found {Count} channels for user {UserId}", channels.Count, userId);
                return channels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting channels for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving channels");
            }
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
        public async Task<IActionResult> JoinChannel(int channelId, [FromBody] int userId)
        {
            try
            {
                // Add logging
                _logger.LogInformation($"Joining channel {channelId} for user {userId}");
                
                var channel = await _context.Channels
                    .Include(c => c.Memberships)
                    .FirstOrDefaultAsync(c => c.Id == channelId);

                if (channel == null)
                {
                    _logger.LogWarning($"Channel {channelId} not found");
                    return NotFound();
                }

                // Check if user is already a member
                if (channel.Memberships?.Any(m => m.UserId == userId) == true)
                {
                    _logger.LogInformation($"User {userId} is already a member of channel {channelId}");
                    return Ok();
                }

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
            catch (Exception ex)
            {
                _logger.LogError($"Error joining channel: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{channelId}/leave")]
        public async Task<ActionResult> LeaveChannel(int channelId, [FromBody] int userId)
        {
            try
            {
                _logger.LogInformation("User {UserId} leaving channel {ChannelId}", userId, channelId);

                var membership = await _context.ChannelMemberships
                    .FirstOrDefaultAsync(m => m.ChannelId == channelId && m.UserId == userId);

                if (membership == null)
                {
                    _logger.LogWarning("Membership not found for user {UserId} in channel {ChannelId}", userId, channelId);
                    return NotFound("User is not a member of this channel");
                }

                _context.ChannelMemberships.Remove(membership);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {UserId} from channel {ChannelId}", userId, channelId);
                return StatusCode(500, "An error occurred while leaving the channel");
            }
        }
    }

    public class CreateChannelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
} 