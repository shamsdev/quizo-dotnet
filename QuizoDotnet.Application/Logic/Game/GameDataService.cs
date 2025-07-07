using Microsoft.Extensions.DependencyInjection;
using QuizoDotnet.Application.Interfaces.Repositories.Game;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
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

    public async Task<UserProfile?> GetUserProfile(long userId)
    {
        using var scope = serviceProvider.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        return await userProfileRepository.GetByUserId(userId);
    }
}