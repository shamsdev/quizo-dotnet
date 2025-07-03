using KarizmaPlatform.Connection.Server.Attributes;
using KarizmaPlatform.Connection.Server.Base;
using QuizoDotnet.Application.Services;

namespace QuizoDotnet.Handlers.RequestHandlers;

[RequestHandler("game")]
public class GameMakeRequestHandler(GameService gameService) : BaseRequestHandler
{
    private long UserId => ConnectionContext.GetAuthorizationId<long>();

    [Action("ready")]
    public void SetReady()
    {
        gameService.UserReady(UserId);
    }

    [Action("answer")]
    public void SetAnswer(long answerId)
    {
        gameService.UserAnswer(UserId, answerId);
    }
}