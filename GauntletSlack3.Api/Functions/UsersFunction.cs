using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Shared.Models;
using GauntletSlack3.Api.Data;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GauntletSlack3.Api.Functions
{
    public class UsersFunction
    {
        private readonly SlackDbContext _context;
        private readonly ILogger<UsersFunction> _logger;

        public UsersFunction(SlackDbContext context, ILogger<UsersFunction> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Function("GetUsers")]
        public async Task<HttpResponseData> GetUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("GetUsers function processing request.");
            
            try
            {
                var users = await _context.Users.ToListAsync();
                _logger.LogInformation("Retrieved {Count} users from database", users.Count);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(users);
                
                _logger.LogInformation("GetUsers function completed successfully");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("An error occurred while processing your request.");
                return response;
            }
        }

        [Function("GetUser")]
        public async Task<HttpResponseData> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{id}")] HttpRequestData req,
            string id)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            var user = await _context.Users.FindAsync(id);
            
            if (user == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            await response.WriteAsJsonAsync(user);
            return response;
        }

        [Function("CreateUser")]
        public async Task<HttpResponseData> CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            var user = await JsonSerializer.DeserializeAsync<User>(req.Body);
            var response = req.CreateResponse(HttpStatusCode.Created);

            if (user == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await response.WriteAsJsonAsync(user);
            return response;
        }

        [Function("DeleteUser")]
        public async Task<HttpResponseData> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "users/{id}")] HttpRequestData req,
            string id)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return response;
        }
    }
} 