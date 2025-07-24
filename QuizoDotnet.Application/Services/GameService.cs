using System.Collections.Concurrent;
using QuizoDotnet.Application.Interfaces;
using QuizoDotnet.Application.Logic.Game;

namespace QuizoDotnet.Application.Services;

public class GameService(
    IServiceProvider serviceProvider,
    IClientCallService callService)
{
    private readonly ConcurrentDictionary<Guid, GameInstance> gamesPool = new();
    private readonly ConcurrentDictionary<long, Guid> userGamesPool = new();

    public void CreateGame(GameUser u1, GameUser u2)
    {
        var gameGuid = Guid.NewGuid();

        var gameInstance = new GameInstance(
            this,
            serviceProvider,
            callService,
            gameGuid,
            u1,
            u2);

        try
        {
            if (!gamesPool.TryAdd(gameGuid, gameInstance))
                throw new Exception($"Error adding game with GUID '{gameGuid}' to the pool.");

            if (!userGamesPool.TryAdd(u1.UserId, gameGuid))
                throw new Exception($"Error adding user with Id '{u1.UserId}' to the pool.");

            if (!userGamesPool.TryAdd(u2.UserId, gameGuid))
                throw new Exception($"Error adding user with Id '{u2.UserId}' to the pool.");
        }
        catch (Exception ex)
        {
            gameInstance.GameClose();
            throw;
        }

        gameInstance.GameStart();
    }

    public void RemoveUser(long userId)
    {
        userGamesPool.TryRemove(userId, out var gameGuid);
        if (gamesPool.TryGetValue(gameGuid, out var gameInstance))
            gameInstance!.RemoveUser(userId);
    }

    public void DisposeGame(GameInstance gameInstance)
    {
        foreach (var user in gameInstance.GameUsers)
        {
            var removeUserResult = userGamesPool.TryRemove(user.UserId, out _);
            Console.WriteLine($"[GameService] Try Remove User {user.UserId} from pool. Result: {removeUserResult}");
        }

        var removeGameResult = gamesPool.TryRemove(gameInstance.Guid, out _);
        Console.WriteLine($"[GameService] Try Remove Game {gameInstance.Guid} from pool. Result: {removeGameResult}");
    }

    private GameInstance? GetUserGameInstance(long userId)
    {
        userGamesPool.TryGetValue(userId, out var gameGuid);
        gamesPool.TryGetValue(gameGuid, out var gameInstance);
        return gameInstance;
    }

    public void UserReady(long userId)
    {
        var userGame = GetUserGameInstance(userId);
        userGame?.UserReady(userId);
    }

    public void UserAnswer(long userId, long answerId)
    {
        var userGame = GetUserGameInstance(userId);
        userGame!.UserAnswer(userId, answerId);
    }
}