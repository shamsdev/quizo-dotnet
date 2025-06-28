using KarizmaPlatform.Connection.Server.Attributes;
using KarizmaPlatform.Connection.Server.Base;
using QuizoDotnet.Application.Services;

namespace QuizoDotnet.Handlers.RequestHandlers;

[RequestHandler("match-make")]
public class MatchMakeRequestHandler(MatchMakeService matchMakeService) : BaseRequestHandler
{
    private long UserId => ConnectionContext.GetAuthorizationId<long>();

    [Action("join")]
    public void Join()
    {
        matchMakeService.Join(UserId, ConnectionContext.ConnectionId);
    }

    [Action("leave")]
    public void Leave()
    {
        matchMakeService.Leave(UserId);
    }
}