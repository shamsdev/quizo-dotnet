using KarizmaPlatform.Connection.Server.Attributes;
using KarizmaPlatform.Connection.Server.Base;
using QuizoDotnet.Application.Services;

namespace QuizoDotnet.Handlers.EventHandlers;

[EventHandler]
public class EventHandler(
    ILogger<EventHandler> logger,
    GameService gameService,
    MatchMakeService matchMakeService) : BaseEventHandler
{
    private long UserId => ConnectionContext.GetAuthorizationId<long>();

    public override Task OnConnected()
    {
        logger.LogInformation($"Connection {ConnectionContext.ConnectionId} connected.");
        return Task.CompletedTask;
    }

    public override Task OnDisconnected(Exception? exception)
    {
        logger.LogInformation($"Connection {ConnectionContext.ConnectionId} disconnected.");
        matchMakeService.Leave(UserId);
        gameService.RemoveUser(UserId);
        return Task.CompletedTask;
    }
}