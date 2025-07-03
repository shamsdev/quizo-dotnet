using KarizmaPlatform.Core.Logic;
using QuizoDotnet.Domain.Models.Questions;

namespace QuizoDotnet.Application.Interfaces.Repositories.Game;

public interface IQuestionRepository : IRepository<Question>
{
    Task<List<Question>> GetRandomQuestions(int count);
}