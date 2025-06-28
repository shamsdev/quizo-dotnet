using Microsoft.Extensions.DependencyInjection;
using QuizoDotnet.Application.Interfaces;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Instances;

public class GameInstance(
    IServiceProvider serviceProvider,
    IClientCallService clientCallService,
    Guid guid,
    GameUserInstance u1,
    GameUserInstance u2)
{
    private readonly object gameLock = new();
    private int roundNumber = 0;

    private readonly Dictionary<long, GameUserInstance> gameUsers = new()
    {
        { u1.UserId, u1 },
        { u2.UserId, u2 }
    };

    public async void Start()
    {
        foreach (var user in gameUsers.Values)
        {
            var opponent = gameUsers.Values.First(x => x.UserId != user.UserId);
            var opponentProfile = await GetUserProfile(opponent.UserId);

            var sendBody = new
            {
                GameGuid = guid,
                Opponent = new
                {
                    UserId = opponent.UserId,
                    Avatar = opponentProfile!.Avatar,
                    DisplayName = opponentProfile.DisplayName
                }
            };

            await clientCallService.Send(user.ConnectionId, "match/start", sendBody);
        }

        //TODO should implement a timeout for players ready and start round
    }

    private async Task<UserProfile?> GetUserProfile(long userId)
    {
        using var scope = serviceProvider.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        return await userProfileRepository.GetByUserId(userId);
    }

    public void SetReady(long userId)
    {
        lock (gameLock)
        {
            gameUsers[userId].SetReady(true);
            Console.WriteLine($"[GameInstance | {guid}] User with Id '{userId}' is ready.");

            var opponent = gameUsers.Values.First(x => x.UserId != userId);
            if (opponent.IsReady)
            {
                Console.WriteLine($"[GameInstance | {guid}] All users are ready. Starting rounds ...");
                StartRound();
            }
        }
    }

    private async void StartRound()
    {
        foreach (var user in gameUsers.Values)
        {
            user.SetReady(false);
            await clientCallService.Send(user.ConnectionId, "match/start-round", 12);
        }
    }
}