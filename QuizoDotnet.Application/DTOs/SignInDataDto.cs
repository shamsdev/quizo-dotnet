namespace QuizoDotnet.Application.DTOs;

public class SignInDataDto
{
    public DateTimeOffset ServerTime { get; init; }

    public UserDataDto UserData { get; init; }
}