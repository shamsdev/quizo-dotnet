namespace QuizoDotnet.Application.DTOs;

public class HomeDataDto
{
    public UserResourceDto UserResource { get; init; } = null!;
    public UserEnergyDto UserEnergy { get; init; } = null!;
}