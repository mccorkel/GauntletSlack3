using Microsoft.AspNetCore.Mvc;
using GauntletSlack3.Shared.Models;
using GauntletSlack3.Api.Data;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { status = "connected" });
        }

        [HttpPost]
        public async Task<ActionResult<Message>> Post(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return Ok(message);
        }

        [HttpGet("{channelId}")]
        public async Task<ActionResult<List<Message>>> Get(int channelId)
        {
            var messages = await _context.Messages
                .Where(m => m.ChannelId == channelId)
                .ToListAsync();
            return Ok(messages);
        }
    }
} 