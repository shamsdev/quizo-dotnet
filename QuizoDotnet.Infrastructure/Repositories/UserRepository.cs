using KarizmaPlatform.Core.Logic;
using Microsoft.EntityFrameworkCore;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Infrastructure.Repositories;

public class UserRepository(QuizoDatabase database) : BaseRepository<User>(database), IUserRepository
{
    public Task<User?> Get(long id, string password, bool asNoTracking = false)
    {
        return (asNoTracking ? database.Users.AsNoTracking() : database.Users)
            .SingleOrDefaultAsync(u =>
                u.Id == id
                && u.Password == password
                && u.DeletedDate == null);
    }
}