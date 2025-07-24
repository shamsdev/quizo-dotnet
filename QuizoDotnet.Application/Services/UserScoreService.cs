using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Services;

public class UserScoreService(IUserScoreRepository userScoreRepository)
{
    public Task<List<UserScore>> GetTopScores(int count)
    {
        return userScoreRepository.GetTopScores(count);
    }

    public async Task AddScore(long userId, int score)
    {
        var userScore = await userScoreRepository.GetUserScore(userId);

        if (userScore != null)
        {
            // User exists — update score
            userScore.Score += score;
            await userScoreRepository.Update(userScore);
        }
        else
        {
            // User doesn't exist — insert new
            userScore = new UserScore
            {
                UserId = userId,
                Score = score
            };

            await userScoreRepository.Add(userScore);
        }
    }
}