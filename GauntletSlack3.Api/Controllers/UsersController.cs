using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using GauntletSlack3.Shared.Models;
using GauntletSlack3.Api.Data;

namespace GauntletSlack3.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly SlackDbContext _context;

        public UsersController(SlackDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return await _context.Users
                .Include(u => u.Memberships)
                .ToListAsync();
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
    }

    public class UserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
} 