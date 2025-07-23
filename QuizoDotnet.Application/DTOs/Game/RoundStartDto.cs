namespace QuizoDotnet.Application.DTOs.Game;

public class RoundStartDto
{
    public int RoundNumber { get; init; }
    public QuestionDto Question { get; init; } = null!;
}