using KarizmaPlatform.Core.Logic;
using QuizoDotnet.Application.Interfaces.Repositories.Game;
using QuizoDotnet.Domain.Models.Questions;

namespace QuizoDotnet.Infrastructure.Repositories.Game;

public class QuestionAnswerRepository(QuizoDatabase database)
    : BaseRepository<QuestionAnswer>(database), IQuestionAnswerRepository
{
}