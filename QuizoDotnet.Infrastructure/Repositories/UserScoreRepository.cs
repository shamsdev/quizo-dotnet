using KarizmaPlatform.Core.Logic;
using Microsoft.EntityFrameworkCore;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Infrastructure.Repositories;

public class UserScoreRepository(QuizoDatabase database)
    : BaseRepository<UserScore>(database), IUserScoreRepository
{
    public Task<UserScore?> GetUserScore(long userId)
    {
        return database.UserScores
            .SingleOrDefaultAsync(us => us.UserId == userId);
    }

    public Task<List<UserScore>> GetTopScores(int count)
    {
        return database.UserScores
            .AsNoTracking()
            .OrderByDescending(x => x.Score)
            .Include(us => us.User)
            .ThenInclude(u => u.UserProfile)
            .Take(count)
            .ToListAsync();
    }
}