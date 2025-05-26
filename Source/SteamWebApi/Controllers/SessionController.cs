using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService sessionService;
        private readonly IUserService userService;

        public SessionController(ISessionService sessionService, IUserService userService)
        {
            this.sessionService = sessionService;
            this.userService = userService;
        }

        [HttpGet("{sessionId}")]
        public IActionResult GetSession(Guid sessionId)
        {
            try
            {
                // Restore the session
                sessionService.RestoreSessionFromDatabase(sessionId);

                // Get the current user
                var user = sessionService.GetCurrentUser();
                if (user == null)
                {
                    return Unauthorized();
                }

                // Return UserWithSessionDetails
                var userWithSessionDetails = new UserWithSessionDetails
                {
                    SessionId = sessionId,
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Developer = user.IsDeveloper,
                    UserCreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin,
                    CreatedAt = UserSession.Instance.CreatedAt,
                    ExpiresAt = UserSession.Instance.ExpiresAt
                };

                return Ok(userWithSessionDetails);
            }
            catch
            {
                return Unauthorized();
            }
        }

        [HttpPost("Logout")]
        [Authorize]
        public IActionResult Logout()
        {
            sessionService.EndSession();
            return Ok();
        }

        [HttpGet("Current")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var user = sessionService.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }
    }
}