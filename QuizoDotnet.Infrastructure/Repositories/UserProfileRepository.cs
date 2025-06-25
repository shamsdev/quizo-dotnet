using KarizmaPlatform.Core.Logic;
using Microsoft.EntityFrameworkCore;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Infrastructure.Repositories;

public class UserProfileRepository(QuizoDatabase database)
    : BaseRepository<UserProfile>(database), IUserProfileRepository
{
    public async Task<UserProfile?> GetByUserId(long userId, bool asNoTracking = false)
    {
        return await (asNoTracking ? database.UserProfiles.AsNoTracking() : database.UserProfiles)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public Task<List<UserProfile>> GetByUserIds(IEnumerable<long> userIds)
    {
        return database.UserProfiles.Where(x => userIds.Contains(x.UserId)).ToListAsync();
    }
}