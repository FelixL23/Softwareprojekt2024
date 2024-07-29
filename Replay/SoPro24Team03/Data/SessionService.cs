
using System.Collections.Concurrent;

namespace SoPro24Team03.Data;

public class SessionService : ISessionService
{
    private readonly ConcurrentDictionary<string, bool> _sessions = new ConcurrentDictionary<string, bool>();

    // Made by Fabio
    // Auth-Session erstellen
    public Task CreateSession(string sessionId)
    {
        _sessions[sessionId] = true;
        return Task.CompletedTask;
    }

    // Made by Fabio
    // Auth-Session invaldieren und aus den Sessiondict löschen
    public Task InvalidateSession(string sessionId)
    {
        _sessions.TryRemove(sessionId, out _);
        return Task.CompletedTask;
    }

    // Made by Fabio
    // Session von benutzern invalidieren
    public Task InvalidateUserSessions(string userId)
    {
        foreach (var sessionId in _sessions.Keys.Where(k => k.StartsWith(userId)))
        {
            _sessions.TryRemove(sessionId, out _);
        }

        return Task.CompletedTask;
    }

    // Made by Fabio
    // Methode um zu überprüfen ob eine Session valide ist
    public Task<bool> IsSessionValid(string sessionId)
    {
        return Task.FromResult(_sessions.TryGetValue(sessionId, out var isValid) && isValid);
    }
}