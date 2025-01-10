using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Shared.Models;
using GauntletSlack3.Api.Data;
using Microsoft.Extensions.Logging;

namespace GauntletSlack3.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly SlackDbContext _context;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(SlackDbContext context, ILogger<MessagesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/messages/channel/{channelId}
        [HttpPost("channel/{channelId}")]
        public async Task<ActionResult<Message>> PostMessage(int channelId, [FromBody] CreateMessageRequest request)
        {
            try
            {
                _logger.LogInformation("Creating message in channel {ChannelId} from user {UserId} with content: {Content}", 
                    channelId, request.UserId, request.Content);

                if (request == null)
                {
                    _logger.LogWarning("Request body is null");
                    return BadRequest("Request body cannot be null");
                }

                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    _logger.LogWarning("Message content is empty");
                    return BadRequest("Message content cannot be empty");
                }

                var channel = await _context.Channels
                    .Include(c => c.Memberships)
                    .FirstOrDefaultAsync(c => c.Id == channelId);

                if (channel == null)
                {
                    _logger.LogWarning("Channel {ChannelId} not found", channelId);
                    return NotFound($"Channel {channelId} not found");
                }

                // Check if user is a member of the channel
                var isMember = await _context.ChannelMemberships
                    .AnyAsync(cm => cm.ChannelId == channelId && cm.UserId == request.UserId);

                if (!isMember)
                {
                    _logger.LogWarning("User {UserId} is not a member of channel {ChannelId}", request.UserId, channelId);
                    return BadRequest("User is not a member of this channel");
                }

                var message = new Message
                {
                    Content = request.Content,
                    UserId = request.UserId,
                    ChannelId = channelId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // Load the user for the response
                await _context.Entry(message)
                    .Reference(m => m.User)
                    .LoadAsync();

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating message in channel {ChannelId}", channelId);
                return StatusCode(500, "An error occurred while creating the message");
            }
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

        // POST: api/messages/{messageId}/reply
        [HttpPost("{messageId}/reply")]
        public async Task<ActionResult<Message>> PostReply(int messageId, [FromBody] CreateMessageRequest request)
        {
            try
            {
                _logger.LogInformation("Creating reply to message {MessageId}", messageId);

                var parentMessage = await _context.Messages
                    .Include(m => m.Channel)
                    .ThenInclude(c => c.Memberships)
                    .FirstOrDefaultAsync(m => m.Id == messageId);

                if (parentMessage == null)
                {
                    _logger.LogWarning("Parent message {MessageId} not found", messageId);
                    return NotFound($"Message {messageId} not found");
                }

                // Check if user is a member of the channel
                var isMember = parentMessage.Channel?.Memberships
                    .Any(m => m.UserId == request.UserId) ?? false;

                if (!isMember)
                {
                    _logger.LogWarning("User {UserId} is not a member of the channel", request.UserId);
                    return BadRequest("User is not a member of this channel");
                }

                var reply = new Message
                {
                    Content = request.Content,
                    UserId = request.UserId,
                    ChannelId = parentMessage.ChannelId,
                    ParentMessageId = messageId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Messages.Add(reply);
                await _context.SaveChangesAsync();

                // Load related data for response
                await _context.Entry(reply)
                    .Reference(m => m.User)
                    .LoadAsync();

                return Ok(reply);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reply to message {MessageId}", messageId);
                return StatusCode(500, "An error occurred while creating the reply");
            }
        }

        // POST: api/messages/{messageId}/reactions
        [HttpPost("{messageId}/reactions")]
        public async Task<ActionResult> AddReaction(int messageId, [FromBody] AddReactionRequest request)
        {
            try
            {
                _logger.LogInformation("Adding reaction to message {MessageId}", messageId);

                var message = await _context.Messages
                    .Include(m => m.Channel)
                    .ThenInclude(c => c.Memberships)
                    .FirstOrDefaultAsync(m => m.Id == messageId);

                if (message == null)
                {
                    return NotFound($"Message {messageId} not found");
                }

                // Check if user is a member of the channel
                var isMember = message.Channel?.Memberships
                    .Any(m => m.UserId == request.UserId) ?? false;

                if (!isMember)
                {
                    return BadRequest("User is not a member of this channel");
                }

                var reaction = new MessageReaction
                {
                    MessageId = messageId,
                    UserId = request.UserId,
                    Emoji = request.Emoji,
                    CreatedAt = DateTime.UtcNow
                };

                _context.MessageReactions.Add(reaction);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding reaction to message {MessageId}", messageId);
                return StatusCode(500, "An error occurred while adding the reaction");
            }
        }

        // DELETE: api/messages/{messageId}/reactions
        [HttpDelete("{messageId}/reactions")]
        public async Task<ActionResult> RemoveReaction(int messageId, [FromQuery] int userId, [FromQuery] string emoji)
        {
            try
            {
                var reaction = await _context.MessageReactions
                    .FirstOrDefaultAsync(r => 
                        r.MessageId == messageId && 
                        r.UserId == userId && 
                        r.Emoji == emoji);

                if (reaction == null)
                {
                    return NotFound("Reaction not found");
                }

                _context.MessageReactions.Remove(reaction);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing reaction from message {MessageId}", messageId);
                return StatusCode(500, "An error occurred while removing the reaction");
            }
        }

        public class CreateMessageRequest
        {
            public string Content { get; set; } = string.Empty;
            public int UserId { get; set; }
        }

        public class AddReactionRequest
        {
            public int UserId { get; set; }
            public string Emoji { get; set; } = string.Empty;
        }
    }
} 