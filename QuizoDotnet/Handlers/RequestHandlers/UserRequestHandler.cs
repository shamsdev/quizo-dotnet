using KarizmaPlatform.Connection.Core.Exceptions;
using KarizmaPlatform.Connection.Server.Attributes;
using KarizmaPlatform.Connection.Server.Base;
using QuizoDotnet.Application.DTOs;
using QuizoDotnet.Application.Services;

namespace QuizoDotnet.Handlers.RequestHandlers;

[RequestHandler("user")]
public class UserRequestHandler(
    UserService userService,
    UserResourceService userResourceService,
    UserEnergyService userEnergyService,
    TokenService tokenService) : BaseRequestHandler
{
    private long UserId => ConnectionContext.GetAuthorizationId<long>();

    [Action("sign-up", needAuthorizedUser: false)]
    public async Task<AccessTokenDto> SignUp()
    {
        var userResult = await userService.SignUp();
        if (!userResult.HasResult)
            throw new ResponseException(100, $"User creation failed with code {userResult.ErrorCode}");

        var accessToken = tokenService.GetAccessToken(
            new TokenService.AccessTokenData
            {
                UserId = userResult.Result!.Id,
                Password = userResult.Result.Password
            });

        return new AccessTokenDto { AccessToken = accessToken };
    }

    [Action("sign-in", needAuthorizedUser: false)]
    public async Task<SignInDataDto> SignIn(AccessTokenDto accessTokenDto)
    {
        var accessTokenData = tokenService.GetAccessTokenData(accessTokenDto.AccessToken);
        var userResult = await userService.SignIn(accessTokenData.UserId, accessTokenData.Password);
        if (!userResult.HasResult)
            throw new ResponseException(100, $"User fetch failed with code {userResult.ErrorCode}");

        var user = userResult.Result!;
        ConnectionContext.SetAuthorizationId(user.Id);

        //TODO profile should delete from here and add to home data
        return new SignInDataDto
        {
            ServerTime = DateTimeOffset.Now,
            UserData = new UserDataDto
            {
                UserId = user.Id,
                UserProfile = new UserProfileDto
                {
                    UserId = user.Id,
                    Avatar = user.UserProfile.Avatar,
                    DisplayName = user.UserProfile.DisplayName
                }
            },
        };
    }

    [Action("update-profile")]
    public async Task<UserProfileDto> UpdateProfile(UserProfileDto userProfileDto)
    {
        var updatedProfileResult =
            await userService.UpdateProfile(UserId, userProfileDto.Avatar, userProfileDto.DisplayName!);

        if (!updatedProfileResult.HasResult)
            throw new ResponseException(100, $"Update profile with code {updatedProfileResult.ErrorCode}");

        var userProfile = updatedProfileResult.Result!;

        return new UserProfileDto
        {
            UserId = userProfile.UserId,
            Avatar = userProfile.Avatar,
            DisplayName = userProfile.DisplayName
        };
    }

    [Action("get-home-data")]
    public async Task<HomeDataDto> GetHomeData()
    {
        var userResourceTask =
            userResourceService.Get(UserId);

        var userEnergyTask =
            userEnergyService.Get(UserId);

        await Task.WhenAll(userResourceTask, userEnergyTask);

        return new HomeDataDto
        {
            UserResource = new UserResourceDto
            {
                Xp = userResourceTask.Result.Xp,
                Coin = userResourceTask.Result.Coin,
            },

            UserEnergy = new UserEnergyDto
            {
                Amount = userEnergyTask.Result.Amount,
                NextGenerationAt = userEnergyTask.Result.NextGenerationAt,
            }
        };
    }
}