namespace QuizoDotnet.Application.Instances;

public class GameUserInstance
{
    public long UserId { get; init; }
    public string ConnectionId { get; init; } = null!;
    public int Score { get; private set; } = 0;
    public bool IsReady { get; private set; }
    public long? AnswerId { get; private set; }
    public bool IsAnswered => AnswerId != null;

    public void AddScore()
    {
        Score++;
    }

    public void SetReady(bool ready)
    {
        IsReady = ready;
    }

    public void SetAnswer(long? answerId)
    {
        AnswerId = answerId;
    }
}