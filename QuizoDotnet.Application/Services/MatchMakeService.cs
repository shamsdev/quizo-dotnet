using System.Collections.Concurrent;

namespace QuizoDotnet.Application.Services;

public class MatchMakeService
{
    private record Requester(long UserId, string ConnectionId);

    private readonly ConcurrentDictionary<long, Requester> matchMakingPool = new();
    private readonly object matchMakingLock = new();

    public void Join(long userId, string connectionId)
    {
        var requester = new Requester(userId, connectionId);

        if (!matchMakingPool.TryAdd(userId, requester))
            return; // Already in pool

        Console.WriteLine($"[MatchMakeService] User with Id '{requester.UserId}' joined match-make.");

        TryMatchMake();
    }

    public void Leave(long userId)
    {
        matchMakingPool.TryRemove(userId, out _);
        Console.WriteLine($"[MatchMakeService] User with Id '{userId}' left match-make.");
    }

    private void TryMatchMake()
    {
        lock (matchMakingLock)
        {
            while (matchMakingPool.Count >= 2)
            {
                var players = matchMakingPool.Take(2).ToArray();

                if (players.Length < 2)
                    return;

                var player1 = players[0].Value;
                var player2 = players[1].Value;

                // Remove matched players
                matchMakingPool.TryRemove(player1.UserId, out _);
                matchMakingPool.TryRemove(player2.UserId, out _);

                StartMatch(player1, player2);
            }
        }
    }

    private void StartMatch(Requester r1, Requester r2)
    {
        Console.WriteLine($"[MatchMakeService] Matched '{r1.UserId}' with '{r2.UserId}'. Starting match ...");
        // TODO: Your match starting logic
    }
}