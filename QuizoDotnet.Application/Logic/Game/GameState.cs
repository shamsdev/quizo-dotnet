using QuizoDotnet.Domain.Models.Questions;

namespace QuizoDotnet.Application.Logic.Game;

public class GameState(Guid guid, GameUser u1, GameUser u2)
{
    public Guid Guid { get; } = guid;
    public int RoundIndex { get; set; } = 0;
    public int RoundNumber => RoundIndex + 1;
    public List<Question>? Questions { get; private set; }
    public int MaxRounds => Questions!.Count;

    public void SetQuestions(List<Question> questions)
    {
        this.Questions = questions;
    }

    public Dictionary<long, GameUser> GameUsers { get; } = new()
    {
        { u1.UserId, u1 },
        { u2.UserId, u2 }
    };
}