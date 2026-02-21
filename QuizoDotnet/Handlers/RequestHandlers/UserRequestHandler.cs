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
        // Sequential to avoid concurrent use of the same scoped DbContext
        var userResource = await userResourceService.Get(UserId);
        var userEnergy = await userEnergyService.CalculateEnergy(UserId);

        return new HomeDataDto
        {
            UserResource = new UserResourceDto
            {
                Xp = userResource.Xp,
                Coin = userResource.Coin,
            },
            UserEnergy = new UserEnergyDto
            {
                Amount = userEnergy.Amount,
                NextGenerationAt = userEnergy.NextGenerationAt,
            }
        };
    }

    [Action("get-energies")]
    public async Task<UserEnergyDto> GetEnergies()
    {
        var userEnergy = await userEnergyService.CalculateEnergy(UserId);
        return new UserEnergyDto
        {
            Amount = userEnergy.Amount,
            NextGenerationAt = userEnergy.NextGenerationAt,
        };
    }
}