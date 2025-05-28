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
    private readonly ISessionService session_service;

    public SessionController(ISessionService sessionService)
    {
        session_service = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    }

    [HttpGet("CurrentUser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await session_service.GetCurrentUserAsync();
        if (user == null)
            return NotFound("No active session found");

        return Ok(user);
    }

    [HttpGet("IsLoggedIn")]
    public async Task<IActionResult> IsLoggedIn()
    {
        return Ok(await session_service.IsUserLoggedInAsync());
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await session_service.EndSessionAsync();
        return Ok();
    }
        
    [HttpGet("Current")]
    public async Task<IActionResult> GetCurrentSession()
    {
        var session = await session_service.GetCurrentSessionDetailsAsync();
        if (session == null)
            return NotFound("No active session found");

        return Ok(session);
    }

    [HttpGet("Validate/{sessionId}")]
    public async Task<IActionResult> ValidateSession(Guid sessionId)
    {
        return Ok(await session_service.ValidateSessionAsync(sessionId));
    }

    [HttpPost("Cleanup")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CleanupExpiredSessions()
    {
        await session_service.CleanupExpiredSessionsAsync();
        return Ok();
    }

    [HttpGet("{sessionId}")]
    public async Task<IActionResult> GetSessionDetails(Guid sessionId)
    {
        var session = await session_service.GetCurrentSessionDetailsAsync();
        if (session == null)
            return NotFound("Session not found");

        return Ok(session);
    }
} 