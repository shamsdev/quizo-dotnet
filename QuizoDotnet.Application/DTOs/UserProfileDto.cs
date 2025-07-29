namespace QuizoDotnet.Application.DTOs;

public class UserProfileDto
{
    public long UserId { get; init; }
    public string Avatar { get; init; } = null!;
    public string? DisplayName { get; init; }
}