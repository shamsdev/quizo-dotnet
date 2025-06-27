using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Application.Results;
using QuizoDotnet.Application.Utils;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IUserProfileRepository userProfileRepository)
{
    public async Task<TResult<User>> SignUp()
    {
        var user = new User
        {
            Password = StringGenerator.GenerateRandomString(32),
            UserProfile = new UserProfile
            {
                Avatar = "1",
            }
        };

        await userRepository.Add(user);

        return new TResult<User>
        {
            Result = user
        };
    }

    public async Task<TResult<User>> SignIn(long id, string password)
    {
        var user = await userRepository.Get(id, password);
        user!.UpdateLastLogin();
        await userRepository.Update(user);
        return new TResult<User> { Result = user };
    }

    public async Task<TResult<UserProfile>> UpdateProfile(long userId, string avatar, string displayName)
    {
        var userProfile = await userProfileRepository.GetByUserId(userId);
        userProfile!.Avatar = avatar;
        userProfile.DisplayName = displayName;
        await userProfileRepository.Update(userProfile);
        return new TResult<UserProfile> { Result = userProfile };
    }
}