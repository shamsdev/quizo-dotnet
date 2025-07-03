using KarizmaPlatform.Core.Logic;
using QuizoDotnet.Application.Interfaces.Repositories.Game;
using QuizoDotnet.Domain.Models.Questions;

namespace QuizoDotnet.Infrastructure.Repositories.Game;

public class CategoryRepository(QuizoDatabase database)
    : BaseRepository<Category>(database), ICategoryRepository
{
}