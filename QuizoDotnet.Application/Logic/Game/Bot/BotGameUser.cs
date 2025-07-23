using QuizoDotnet.Application.Services;

namespace QuizoDotnet.Application.Logic.Game.Bot;

public class BotGameUser : GameUser
{
    private readonly GameService gameService;
    private readonly Dictionary<string, Func<object, Task>> handlers = new();

    public BotGameUser(GameService gameService)
    {
        this.gameService = gameService;
        InitHandlers();
    }

    private void InitHandlers()
    {
        handlers.Clear();
        handlers.Add(GameBroadcaster.MatchStartCommand, OnMatchStarted);
        handlers.Add(GameBroadcaster.GetReadyCommand, OnGetReady);
        handlers.Add(GameBroadcaster.RoundResultCommand, OnRoundResult);
        handlers.Add(GameBroadcaster.StartRoundCommand, OnStartRound);
    }

    public async void Receive(string address, object body)
    {
        Console.WriteLine($"[Bot] Receiving: {address}");

        if (handlers.TryGetValue(address, out var handler))
            await handler.Invoke(body);
        else
            Console.WriteLine($"[Bot] No handler for address: {address}");
    }

    private Task OnMatchStarted(object body)
    {
        Console.WriteLine("[Bot] OnMatchStarted");
        gameService.UserReady(UserId);
        return Task.CompletedTask;
    }

    private Task OnStartRound(object body)
    {
        Console.WriteLine("[Bot] OnStartRound", body);
        return Task.CompletedTask;
    }

    private Task OnRoundResult(object arg)
    {
        Console.WriteLine("[Bot] OnRoundResult");
        return Task.CompletedTask;
    }

    private Task OnGetReady(object arg)
    {
        Console.WriteLine("[Bot] OnGetReady");
        gameService.UserReady(UserId);
        return Task.CompletedTask;
    }
}