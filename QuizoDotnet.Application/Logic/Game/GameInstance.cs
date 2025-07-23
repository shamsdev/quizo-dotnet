using QuizoDotnet.Application.Interfaces;

// ReSharper disable InconsistentlySynchronizedField

namespace QuizoDotnet.Application.Logic.Game;

public class GameInstance
{
    private readonly object gameLock = new();

    private readonly GameState gameState;
    private readonly GameBroadcaster gameBroadcaster;
    private readonly GameDataService gameDataService;
    private readonly GameTimerService gameTimerService;

    #region Constants and Configs

    private const int RoundCount = 2;
    private const int StartRoundDelayMs = 2 * 1000;
    private const int ShowResultDelayMs = 2 * 1000;
    private const int ShowResultDurationMs = 3 * 1000;
    private const int RoundTimeMs = 5 * 1000;

    #endregion

    public GameUser[] GameUsers => gameState.GameUsers.Values.ToArray();

    public GameInstance(
        IServiceProvider serviceProvider,
        IClientCallService clientCallService,
        Guid guid,
        GameUser u1,
        GameUser u2)
    {
        gameState = new GameState(guid, u1, u2);
        gameBroadcaster = new GameBroadcaster(clientCallService, gameState);
        gameDataService = new GameDataService(serviceProvider);
        gameTimerService = new GameTimerService();
    }

    public async void GameStart()
    {
        Console.WriteLine($"[GameInstance | {gameState.Guid}] Game started.");

        var questions = await gameDataService.PrepareQuestions(RoundCount);
        gameState.SetQuestions(questions);

        foreach (var user in gameState.GameUsers.Values)
        {
            var opponent = gameState.GameUsers.Values.First(x => x.UserId != user.UserId);
            var opponentProfile = await gameDataService.GetUserProfile(opponent);

            var sendBody = new
            {
                GameGuid = gameState.Guid,
                gameState.MaxRounds,
                Opponent = new
                {
                    UserId = opponent.UserId,
                    Avatar = opponentProfile!.Avatar,
                    DisplayName = opponentProfile.DisplayName
                }
            };

            Console.WriteLine($"[GameInstance | {gameState.Guid}] sending match start to '{user.UserId}'.");
            gameBroadcaster.SendMatchStart(sendBody, user.UserId);
        }
    }

    public void UserReady(long userId)
    {
        lock (gameLock)
        {
            gameState.GameUsers[userId].SetReady(true);
            Console.WriteLine($"[GameInstance | {gameState.Guid}] User with Id '{userId}' is ready.");

            var opponent = gameState.GameUsers.Values.First(x => x.UserId != userId);
            if (opponent.IsReady)
            {
                Console.WriteLine($"[GameInstance | {gameState.Guid}] All users are ready. Starting rounds ...");
                gameTimerService.ScheduleJobAsync(StartRoundDelayMs, RoundStart);
            }
        }
    }

    private Task RoundStart()
    {
        Console.WriteLine($"[GameInstance | {gameState.Guid}] Round {gameState.RoundNumber} started.");

        foreach (var user in gameState.GameUsers.Values)
            user.SetReady(false);

        var question = gameState.Questions![gameState.RoundIndex];

        var data = new
        {
            Question = new
            {
                Category = question.Category.Title,
                Title = question.Title,
                Answers = question.Answers.Select(a => new
                {
                    Id = a.Id,
                    Title = a.Title
                })
            },
            gameState.RoundNumber
        };

        gameBroadcaster.SendRoundStart(data);
        gameTimerService.ScheduleJobAsync(RoundTimeMs, RoundResult);
        return Task.CompletedTask;
    }

    public void UserAnswer(long userId, long answerId)
    {
        lock (gameLock)
        {
            gameState.GameUsers[userId].SetAnswer(answerId);

            Console.WriteLine(
                $"[GameInstance | {gameState.Guid}] User with Id '{userId}' answered with id '{answerId}'.");

            var opponent = gameState.GameUsers.Values.First(x => x.UserId != userId);
            if (opponent.IsAnswered)
            {
                Console.WriteLine($"[GameInstance | {gameState.Guid}] All users are answered.");
                gameTimerService.ScheduleJobAsync(ShowResultDelayMs, RoundResult);
            }
        }
    }

    private Task RoundResult()
    {
        Console.WriteLine($"[GameInstance | {gameState.Guid}] Round {gameState.RoundNumber} result.");

        //Calculating answers
        var question = gameState.Questions![gameState.RoundIndex];
        var correctAnswerId = question.Answers.Single(a => a.IsCorrect).Id;
        foreach (var user in gameState.GameUsers.Values)
        {
            if (user.IsAnswered && user.AnswerId == correctAnswerId)
                user.AddScore();
            user.SetAnswer(null);
        }

        //Create data body
        var data = new
        {
            AnswerId = correctAnswerId,
            UsersData = gameState.GameUsers.Values.Select(u => new
                {
                    UserId = u.UserId,
                    AnswerId = u.AnswerId,
                    Score = u.Score
                })
                .ToList()
        };

        //Send Data
        gameBroadcaster.SendRoundResult(data);

        //Finish Round
        gameTimerService.ScheduleJobAsync(ShowResultDurationMs, RoundFinish);

        return Task.CompletedTask;
    }

    private Task RoundFinish()
    {
        Console.WriteLine($"[GameInstance | {gameState.Guid}] round {gameState.RoundNumber} finished.");

        gameState.RoundIndex++;

        if (gameState.RoundIndex >= gameState.MaxRounds)
            GameFinish();
        else gameBroadcaster.RequestGetReady();

        return Task.CompletedTask;
    }

    private void GameFinish()
    {
        Console.WriteLine($"[GameInstance | {gameState.Guid}] Game finished.");
        GameClose();
    }

    public void RemoveUser(long userId)
    {
        Console.WriteLine($"[GameInstance | {gameState.Guid}] User {userId} removed.");
        gameState.GameUsers.Remove(userId);
        //TODO send opponent finish data
        GameClose();
    }

    public void GameClose()
    {
        Console.WriteLine($"[GameInstance | {gameState.Guid}] Game closed.");
        gameTimerService.DisposeTimer();
    }
}