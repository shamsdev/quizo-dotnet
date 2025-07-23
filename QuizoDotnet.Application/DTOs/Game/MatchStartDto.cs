namespace QuizoDotnet.Application.DTOs.Game;

public class MatchStartDto
{
    public Guid GameGuid { get; init; }
    public int MaxRounds { get; init; }
    public UserDataDto Opponent { get; init; } = null!;
    public int QuestionTime { get; init; }
}