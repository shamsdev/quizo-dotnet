namespace QuizoDotnet.Application.DTOs.Game;

public class QuestionDto
{
    public string Category { get; init; } = null!;
    public string Title { get; init; } = null!;
    public List<AnswerDto> Answers { get; init; } = null!;
}