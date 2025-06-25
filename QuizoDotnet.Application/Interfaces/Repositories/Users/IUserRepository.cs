using KarizmaPlatform.Core.Logic;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Interfaces.Repositories.Users;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> Get(long id, string password, bool asNoTracking = false);
}