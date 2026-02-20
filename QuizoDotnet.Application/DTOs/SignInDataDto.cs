namespace QuizoDotnet.Application.DTOs;

public class SignInDataDto
{
    public DateTimeOffset ServerTime { get; init; }
    
    public UserDataDto UserData { get; init; }
    public UserResourceDto UserResource { get; init; }
    public UserEnergyDto UserEnergy { get; init; }
}