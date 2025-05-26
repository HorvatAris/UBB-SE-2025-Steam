using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BusinessLayer.Services.Interfaces;

namespace SteamProfileWeb.Middleware
{
    public class SessionRestorationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionRestorationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var sessionIdClaim = context.User.FindFirstValue("SessionId");
                if (!string.IsNullOrEmpty(sessionIdClaim) && Guid.TryParse(sessionIdClaim, out Guid sessionId))
                {
                    sessionService.RestoreSessionFromDatabase(sessionId);
                }
            }

            await _next(context);
        }
    }

    public static class SessionRestorationMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionRestoration(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionRestorationMiddleware>();
        }
    }
}