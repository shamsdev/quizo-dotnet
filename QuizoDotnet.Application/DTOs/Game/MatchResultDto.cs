namespace QuizoDotnet.Application.DTOs.Game;

public class MatchResultDto
{
    public enum State
    {
        Win = 1,
        Lose = 2,
        Draw = 3,
    }

    public State MatchState { get; init; }
    public int Score { get; init; }
    public string MatchStateStr => MatchState.ToString();
    public bool OpponentLeft { get; set; }
}