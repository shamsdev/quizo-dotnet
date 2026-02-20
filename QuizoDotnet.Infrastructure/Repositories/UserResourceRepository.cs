using KarizmaPlatform.Core.Logic;
using Microsoft.EntityFrameworkCore;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Infrastructure.Repositories;

public class UserResourceRepository(QuizoDatabase database)
    : BaseRepository<UserResource>(database), IUserResourceRepository
{
    public async Task<UserResource> Get(long userId)
    {
        var userResource = await database.UserResources.SingleOrDefaultAsync(x => x.UserId == userId);
        
        if (userResource != null) 
            return userResource;
        
        // Create default
        userResource = new UserResource
        {
            UserId = userId,
            Xp = 0,
            Coin = 100
        };

        database.UserResources.Add(userResource);
        await database.SaveChangesAsync();

        return userResource;
    }
}