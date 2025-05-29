using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SteamHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly IUserService _userService;

    public SessionController(ISessionService sessionService, IUserService userService)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet("CurrentUser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _sessionService.GetCurrentUserAsync();
        var fullUser = _userService.GetUserByIdentifierAsync(user.UserId);
        user.Password = fullUser.Result.Password;
        user.ProfilePicture = fullUser.Result.ProfilePicture;
        if (user == null)
            return NotFound("No active session found");

        return Ok(user);
    }

    [HttpGet("IsLoggedIn")]
    public async Task<IActionResult> IsLoggedIn()
    {
        return Ok(await _sessionService.IsUserLoggedInAsync());
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _sessionService.EndSessionAsync();
        return Ok();
    }
        
    [HttpGet("Current")]
    public async Task<IActionResult> GetCurrentSession()
    {
        var session = await _sessionService.GetCurrentSessionDetailsAsync();
        if (session == null)
            return NotFound("No active session found");

        return Ok(session);
    }

    [HttpGet("Validate/{sessionId}")]
    public async Task<IActionResult> ValidateSession(Guid sessionId)
    {
        return Ok(await _sessionService.ValidateSessionAsync(sessionId));
    }

    [HttpPost("Cleanup")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CleanupExpiredSessions()
    {
        await _sessionService.CleanupExpiredSessionsAsync();
        return Ok();
    }

    [HttpGet("{sessionId}")]
    public async Task<IActionResult> GetSessionDetails(Guid sessionId)
    {
        var session = await _sessionService.GetCurrentSessionDetailsAsync();
        if (session == null)
            return NotFound("Session not found");

        return Ok(session);
    }
} 