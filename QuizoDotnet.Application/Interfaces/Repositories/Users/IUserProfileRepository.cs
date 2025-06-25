using KarizmaPlatform.Core.Logic;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Interfaces.Repositories.Users;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> GetByUserId(long userId, bool asNoTracking = false);
    Task<List<UserProfile>> GetByUserIds(IEnumerable<long> userIds);
}