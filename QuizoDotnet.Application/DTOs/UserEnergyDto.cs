namespace QuizoDotnet.Application.DTOs;

public class UserEnergyDto
{
    public int Amount { get; init; }
    public DateTimeOffset NextGenerationAt { get; init; }
}