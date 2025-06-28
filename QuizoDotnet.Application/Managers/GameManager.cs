using System.Collections.Concurrent;
using QuizoDotnet.Application.Instances;
using QuizoDotnet.Application.Interfaces;

namespace QuizoDotnet.Application.Managers;

public class GameManager
{
    private static readonly ConcurrentDictionary<Guid, GameInstance> GamesPool = new();

    public static void CreateGame(IServiceProvider serviceProvider, IClientCallService callService, GameUserInstance u1,
        GameUserInstance u2)
    {
        var gameGuid = Guid.NewGuid();
        var gameInstance = new GameInstance(serviceProvider, callService, gameGuid, u1, u2);

        if (!GamesPool.TryAdd(gameGuid, gameInstance))
            throw new Exception($"Error adding game with GUID '{gameGuid}' to the pool.");

        gameInstance.Start();
    }
}