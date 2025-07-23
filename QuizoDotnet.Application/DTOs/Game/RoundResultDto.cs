namespace QuizoDotnet.Application.DTOs.Game;

public class RoundResultDto
{
    public long CorrectAnswerId { get; init; }
    public List<UserRoundResultDto> UsersData { get; init; } = null!;
}