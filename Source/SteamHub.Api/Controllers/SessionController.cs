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

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    }

    [HttpGet("CurrentUser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _sessionService.GetCurrentUserAsync();
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