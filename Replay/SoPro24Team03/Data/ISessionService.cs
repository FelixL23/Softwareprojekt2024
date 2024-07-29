namespace SoPro24Team03.Data;

public interface ISessionService
{
    Task CreateSession(string sessionId);
    Task<bool> IsSessionValid(string sessionId);
    Task InvalidateSession(string sessionId);
    Task InvalidateUserSessions(string userId);
}