using QuizoDotnet.Application.Interfaces;

namespace QuizoDotnet.Application.Logic.Game;

public class GameBroadcaster(IClientCallService clientCallService, GameState gameState)
{
    #region Commands

    private const string MatchStartCommand = "match/start";
    private const string GetReadyCommand = "match/get-ready";
    private const string StartRoundCommand = "match/start-round";
    private const string RoundResultCommand = "match/round-result";

    #endregion

    private void SendAll(string address, object body)
    {
        foreach (var user in gameState.GameUsers.Values)
            clientCallService.Send(user.ConnectionId, address, body);
    }

    public void SendMatchStart(object body, long userId)
    {
        var user = gameState.GameUsers[userId];
        clientCallService.Send(user.ConnectionId, MatchStartCommand, body);
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