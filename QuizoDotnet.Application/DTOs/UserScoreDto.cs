namespace QuizoDotnet.Application.DTOs;

public class UserScoreDto
{
    public UserProfileDto UserProfile { get; set; } = null!;
    public int Score { get; set; }
}