using System.Collections.Concurrent;
using QuizoDotnet.Application.Logic.Game;
using QuizoDotnet.Application.Logic.Game.Bot;

namespace QuizoDotnet.Application.Services;

public class MatchMakeService(GameService gameService)
{
    private record Requester(long UserId, string ConnectionId);

    private readonly ConcurrentDictionary<long, Requester> matchMakingPool = new();
    private readonly object matchMakingLock = new();

    private CancellationTokenSource? matchBotCancellationTokenSource;
    private const int MatchBotAfterSeconds = 8;

    public void Join(long userId, string connectionId)
    {
        var requester = new Requester(userId, connectionId);

        lock (matchMakingLock)
        {
            if (!matchMakingPool.TryAdd(userId, requester))
                return; // Already in pool
        }

        Console.WriteLine($"[MatchMakeService] User with Id '{requester.UserId}' joined match-make.");
        TryMatchMake();
    }

    public void Leave(long userId)
    {
        lock (matchMakingLock)
        {
            if (matchMakingPool.TryRemove(userId, out _))
                Console.WriteLine($"[MatchMakeService] User with Id '{userId}' left match-make.");

            if (matchMakingPool.IsEmpty)
                matchBotCancellationTokenSource?.Cancel();
        }
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

        TryBotMatchMake();
    }

    private async void TryBotMatchMake()
    {
        matchBotCancellationTokenSource?.Cancel();
        matchBotCancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(MatchBotAfterSeconds * 1000, matchBotCancellationTokenSource.Token);

            lock (matchMakingLock)
            {
                var (_, requester) = matchMakingPool.ElementAtOrDefault(0);

                if (requester == null)
                    return;

                matchMakingPool.TryRemove(requester.UserId, out _);

                CreateBotGame(requester);
            }
        }
        catch
        {
            // ignored
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

        var bot = new BotGameUser(gameService)
        {
            UserId = -requester.UserId,
            ConnectionId = Guid.NewGuid().ToString()
        };

        gameService.CreateGame(user, bot);
    }
}