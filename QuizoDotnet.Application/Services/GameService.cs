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

        var gameInstance = new GameInstance(serviceProvider,
            callService,
            gameGuid,
            u1,
            u2);

        if (!gamesPool.TryAdd(gameGuid, gameInstance)
            || !userGamesPool.TryAdd(u1.UserId, gameGuid)
            || !userGamesPool.TryAdd(u2.UserId, gameGuid))
        {
            gameInstance.GameClose();
            throw new Exception($"Error adding game with GUID '{gameGuid}' to the pool.");
        }

        gameInstance.GameStart();
    }

    public void RemoveUser(long userId)
    {
        userGamesPool.TryGetValue(userId, out var gameGuid);
        if (gamesPool.TryGetValue(gameGuid, out var gameInstance))
        {
            gameInstance.RemoveUser(userId);
            gamesPool.TryRemove(gameGuid, out _);
        }

        userGamesPool.TryRemove(userId, out _);
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