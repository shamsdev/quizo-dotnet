using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Services;

public class UserResourceService(IUserResourceRepository userResourceRepository)
{
    public async Task<UserResource> Get(long userId)
    {
        return await userResourceRepository.Get(userId);
    }
    
    public async Task UpdateCoin(long userId, int amount)
    {
        var userResource = await userResourceRepository.Get(userId);
        
        if (userResource.Coin + amount < 0)
            throw new InvalidOperationException($"User with id {userId} does not have enough coins");
        
        userResource.Coin += amount;
        await userResourceRepository.Update(userResource);
    }
}