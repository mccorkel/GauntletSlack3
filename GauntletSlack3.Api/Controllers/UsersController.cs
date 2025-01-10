using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using GauntletSlack3.Shared.Models;
using GauntletSlack3.Api.Data;
using Microsoft.Extensions.Logging;

namespace GauntletSlack3.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly SlackDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(SlackDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Memberships)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, "An error occurred while retrieving users");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Memberships)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost("getorcreate")]
        public async Task<ActionResult<int>> GetOrCreateUser([FromBody] UserInfo userInfo)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userInfo.Email);

            if (user == null)
            {
                user = new User
                {
                    Email = userInfo.Email,
                    Name = userInfo.Name,
                    IsAdmin = false
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return user.Id;
        }

        [HttpPut("{userId}/status")]
        public async Task<IActionResult> UpdateStatus(int userId, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                user.IsOnline = request.IsOnline;
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for user {UserId}", userId);
                return StatusCode(500, "An error occurred while updating user status");
            }
        }
    }

    public class UserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateStatusRequest
    {
        public bool IsOnline { get; set; }
    }
} 