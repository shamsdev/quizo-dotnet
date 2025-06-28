namespace QuizoDotnet.Application.Instances;

public class GameUserInstance
{
    public long UserId { get; init; }
    public string ConnectionId { get; init; } = null!;
    public int Score { get; private set; }
}