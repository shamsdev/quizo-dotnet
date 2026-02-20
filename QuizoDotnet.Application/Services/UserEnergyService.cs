using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Services;

public class UserEnergyService(IUserEnergyRepository userEnergyRepository)
{
    public async Task<UserEnergy> CalculateEnergy(long userId)
    {
        var userEnergy = await userEnergyRepository.Get(userId);

        if (userEnergy.Amount >= UserEnergy.MAX_ENERGY)
        {
            userEnergy.LastEnergyUpdatedAt = DateTimeOffset.UtcNow;
            await userEnergyRepository.Update(userEnergy);
            return userEnergy;
        }

        var now = DateTimeOffset.UtcNow;
        var secondsPassed = (now - userEnergy.LastEnergyUpdatedAt).TotalSeconds;

        var energyToAdd = (int)(secondsPassed / UserEnergy.SECONDS_PER_ENERGY);

        if (energyToAdd > 0)
        {
            userEnergy.Amount = Math.Min(UserEnergy.MAX_ENERGY, userEnergy.Amount + energyToAdd);

            userEnergy.LastEnergyUpdatedAt = userEnergy.LastEnergyUpdatedAt
                .AddSeconds(energyToAdd * UserEnergy.SECONDS_PER_ENERGY);

            await userEnergyRepository.Update(userEnergy);
        }

        return userEnergy;
    }

    public async Task Consume(long userId)
    {
        var userEnergy = await CalculateEnergy(userId);

        if (userEnergy.Amount <= 0)
            throw new InvalidOperationException($"User with id {userId} does not have enough energy to consume.");

        userEnergy.Amount--;
        await userEnergyRepository.Update(userEnergy);
    }
}