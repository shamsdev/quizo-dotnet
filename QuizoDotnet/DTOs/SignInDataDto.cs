namespace QuizoDotnet.DTOs;

public class SignInDataDto
{
    public UserDataDto UserData { get; init; }
    public DateTimeOffset ServerTime { get; init; }
}