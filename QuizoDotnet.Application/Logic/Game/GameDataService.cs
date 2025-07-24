using Microsoft.Extensions.DependencyInjection;
using QuizoDotnet.Application.Interfaces.Repositories.Game;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Application.Logic.Game.Bot;
using QuizoDotnet.Application.Services;
using QuizoDotnet.Domain.Models.Questions;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Logic.Game;

public class GameDataService(IServiceProvider serviceProvider)
{
    public async Task<List<Question>> PrepareQuestions(int questionsCount)
    {
        using var scope = serviceProvider.CreateScope();
        var questionRepository = scope.ServiceProvider.GetRequiredService<IQuestionRepository>();
        return await questionRepository.GetRandomQuestions(questionsCount);
    }

    public async Task<UserProfile?> GetUserProfile(GameUser user)
    {
        if (user is BotGameUser)
        {
            return new UserProfile
            {
                Avatar = "40",
                DisplayName = "Bot",
                UserId = user.UserId
            };
        }

        using var scope = serviceProvider.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        return await userProfileRepository.GetByUserId(user.UserId);
    }

    public async Task AddScore(long userId, int score)
    {
        using var scope = serviceProvider.CreateScope();
        var userScoreService = scope.ServiceProvider.GetRequiredService<UserScoreService>();
        await userScoreService.AddScore(userId, score);
    }
}