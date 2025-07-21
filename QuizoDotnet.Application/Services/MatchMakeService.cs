using System.Collections.Concurrent;
using QuizoDotnet.Application.Logic.Game;
using QuizoDotnet.Application.Logic.Game.Bot;

namespace QuizoDotnet.Application.Services;

public class MatchMakeService(GameService gameService)
{
    private record Requester(long UserId, string ConnectionId);

    private readonly ConcurrentDictionary<long, Requester> matchMakingPool = new();
    private readonly object matchMakingLock = new();

    private const bool UseBot = true;

    public void Join(long userId, string connectionId)
    {
        var requester = new Requester(userId, connectionId);

        if (UseBot)
        {
            CreateBotGame(requester);
            return;
        }

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

                CreateGame(player1, player2);
            }
        }
    }

    private void CreateGame(Requester r1, Requester r2)
    {
        Console.WriteLine($"[MatchMakeService] Matched '{r1.UserId}' with '{r2.UserId}'. Creating game ...");

        var u1 = new GameUser
        {
            UserId = r1.UserId,
            ConnectionId = r1.ConnectionId
        };

        var u2 = new GameUser
        {
            UserId = r2.UserId,
            ConnectionId = r2.ConnectionId
        };

        gameService.CreateGame(u1, u2);
    }

    private void CreateBotGame(Requester requester)
    {
        Console.WriteLine($"[MatchMakeService] Matched '{requester.UserId}' with bot. Creating game ...");

        var user = new GameUser
        {
            UserId = requester.UserId,
            ConnectionId = requester.ConnectionId
        };

        var bot = new BotGameUser
        {
            UserId = -requester.UserId,
            ConnectionId = Guid.NewGuid().ToString()
        };

        gameService.CreateGame(user, bot);
    }
}