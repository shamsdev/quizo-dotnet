namespace QuizoDotnet.Application.DTOs.Game;

public class UserRoundResultDto
{
    public long UserId { get; init; }
    public long? AnswerId { get; init; }
    public int Score { get; init; }
}