using KarizmaPlatform.Core.Logic;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Interfaces.Repositories.Users;

public interface IUserScoreRepository : IRepository<UserScore>
{
    Task<UserScore?> GetUserScore(long userId);
    Task<List<UserScore>> GetTopScores(int count);
}