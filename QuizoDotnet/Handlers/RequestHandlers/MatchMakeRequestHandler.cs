using KarizmaPlatform.Connection.Core.Exceptions;
using KarizmaPlatform.Connection.Server.Attributes;
using KarizmaPlatform.Connection.Server.Base;
using QuizoDotnet.Application.Services;

namespace QuizoDotnet.Handlers.RequestHandlers;

[RequestHandler("match-make")]
public class MatchMakeRequestHandler(
    MatchMakeService matchMakeService,
    UserEnergyService userEnergyService)
    : BaseRequestHandler
{
    private long UserId => ConnectionContext.GetAuthorizationId<long>();

    [Action("join")]
    public async Task Join()
    {
        var userEnergy = await userEnergyService.CalculateEnergy(UserId);
        if (userEnergy.Amount <= 0)
            throw new ResponseException(107, "User with id {userId} does not have enough energy to consume.");
       
        matchMakeService.Join(UserId, ConnectionContext.ConnectionId);
    }

    [Action("leave")]
    public void Leave()
    {
        matchMakeService.Leave(UserId);
    }
}