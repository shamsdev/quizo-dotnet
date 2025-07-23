using QuizoDotnet.Application.Interfaces;
using QuizoDotnet.Application.Logic.Game.Bot;

namespace QuizoDotnet.Application.Logic.Game;

public class GameBroadcaster(IClientCallService clientCallService, GameState gameState)
{
    #region Commands

    public const string MatchStartCommand = "match/start";
    public const string GetReadyCommand = "match/get-ready";
    public const string StartRoundCommand = "match/start-round";
    public const string RoundResultCommand = "match/round-result";

    #endregion

    private void Send(GameUser user, string address, object body)
    {
        if (user is BotGameUser botUser)
            botUser.Receive(address, body);
        else
            clientCallService.Send(user.ConnectionId, address, body);
    }

    private void SendAll(string address, object body)
    {
        foreach (var user in gameState.GameUsers.Values)
            Send(user, address, body);
    }

    public void SendMatchStart(object body, long userId)
    {
        var user = gameState.GameUsers[userId];
        Send(user, MatchStartCommand, body);
    }

    public void RequestGetReady()
    {
        SendAll(GetReadyCommand, new { gameState.RoundNumber });
    }

    public void SendRoundStart(object data)
    {
        SendAll(StartRoundCommand, data);
    }

    public void SendRoundResult(object data)
    {
        SendAll(RoundResultCommand, data);
    }
}