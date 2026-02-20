using KarizmaPlatform.Core.Logic;
using Microsoft.EntityFrameworkCore;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Infrastructure.Repositories;

public class UserEnergyRepository(QuizoDatabase database)
    : BaseRepository<UserEnergy>(database), IUserEnergyRepository
{
    public async Task<UserEnergy> Get(long userId)
    {
        var userEnergy = await database.UserEnergies.SingleOrDefaultAsync(x => x.UserId == userId && x.DeletedDate == null);
        
        if (userEnergy != null) 
            return userEnergy;
        
        // Create default
        userEnergy = new UserEnergy
        {
            UserId = userId,
            Amount = UserEnergy.MAX_ENERGY,
            LastEnergyUpdatedAt = DateTimeOffset.UtcNow,
        };

        database.UserEnergies.Add(userEnergy);
        await database.SaveChangesAsync();

        return userEnergy;
    }
}