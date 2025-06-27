using KarizmaPlatform.Core.Logic;
using Microsoft.EntityFrameworkCore;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Infrastructure.Repositories;

public class UserRepository(QuizoDatabase database) : BaseRepository<User>(database), IUserRepository
{
    public Task<User?> Get(long id, string password, bool includeProfile = true, bool asNoTracking = false)
    {
        var users = (asNoTracking ? database.Users.AsNoTracking() : database.Users);

        if (includeProfile)
            users = users.Include(u => u.UserProfile);

        return users.SingleOrDefaultAsync(u =>
            u.Id == id
            && u.Password == password
            && u.DeletedDate == null);
    }
}