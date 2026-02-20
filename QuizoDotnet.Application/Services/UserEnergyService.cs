using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Services;

public class UserEnergyService(IUserEnergyRepository userEnergyRepository)
{
    public async Task<UserEnergy> Get(long userId)
    {
        return await userEnergyRepository.Get(userId);
    }
    
    private async Task<UserEnergy> CalculateEnergy(long userId)
    {
        var userEnergy = await userEnergyRepository.Get(userId);

        if (userEnergy.Amount >= UserEnergy.MAX_ENERGY)
        {
            userEnergy.LastEnergyUpdatedAt = DateTime.UtcNow;
            await userEnergyRepository.Update(userEnergy);
            return userEnergy;
        }

        var now = DateTime.UtcNow;
        var secondsPassed = (now - userEnergy.LastEnergyUpdatedAt).TotalSeconds;

        var energyToAdd = (int)(secondsPassed / UserEnergy.SECONDS_PER_ENERGY);

        if (energyToAdd > 0)
        {
            userEnergy.Amount = Math.Min(UserEnergy.MAX_ENERGY, userEnergy.Amount + energyToAdd);

            // Move forward only the consumed time
            userEnergy.LastEnergyUpdatedAt = userEnergy.LastEnergyUpdatedAt
                .AddMinutes(energyToAdd * UserEnergy.SECONDS_PER_ENERGY);

            await userEnergyRepository.Update(userEnergy);
        }

        return userEnergy;
    }

    public async Task Consume(long userId)
    {
        var userEnergy = await CalculateEnergy(userId);

        if (userEnergy.Amount <= 0)
            throw new Exception($"User with id {userId} does not have enough energy to consume.");

        userEnergy.Amount--;
        await userEnergyRepository.Update(userEnergy);
    }
}