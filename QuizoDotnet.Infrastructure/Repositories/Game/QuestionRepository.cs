using KarizmaPlatform.Core.Logic;
using QuizoDotnet.Application.Interfaces.Repositories.Game;
using QuizoDotnet.Domain.Models.Questions;
using Microsoft.EntityFrameworkCore;

namespace QuizoDotnet.Infrastructure.Repositories.Game;

public class QuestionRepository(QuizoDatabase database)
    : BaseRepository<Question>(database), IQuestionRepository
{
    public async Task<List<Question>> GetRandomQuestions(int count)
    {
        return await database.Questions
            .OrderBy(q => Guid.NewGuid())
            .Include(q => q.Answers)
            .Include(q => q.Category)
            .Take(count)
            .ToListAsync();
    }
}