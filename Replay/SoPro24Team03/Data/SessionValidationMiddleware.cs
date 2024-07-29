using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SoPro24Team03.Data;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ISessionService _sessionService;

    public SessionValidationMiddleware(RequestDelegate next, ISessionService sessionService)
    {
        _next = next;
        _sessionService = sessionService;
    }

    // Made by Fabio
    // Methode um bei jedem Click überprüfen ob der aktuelle Benutzer die Seite benutzen darg
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("SessionId", out var sessionId))
        {
            if (!await _sessionService.IsSessionValid(sessionId))
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
             
                context.Response.Cookies.Delete("SessionId");

                context.Response.Redirect("/Authentication");

                return;
            }
        }

        await _next(context);
    }
}