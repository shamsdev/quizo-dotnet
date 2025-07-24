using QuizoDotnet.Application.DTOs;
using QuizoDotnet.Application.DTOs.Game;
using QuizoDotnet.Application.Interfaces;
using QuizoDotnet.Application.Logic.Game.Bot;
using QuizoDotnet.Application.Services;

// ReSharper disable InconsistentlySynchronizedField

namespace QuizoDotnet.Application.Logic.Game;

public class GameInstance
{
    public readonly Guid Guid;
    private readonly GameService gameService;

    private readonly GameState gameState;
    private readonly GameBroadcaster gameBroadcaster;
    private readonly GameDataService gameDataService;
    private readonly GameTimerService gameTimerService;

    private readonly object gameLock = new();

    public GameUser[] GameUsers => gameState.GameUsersDict.Values.ToArray();

    #region Constants and Configs

    private const int RoundCount = 2;
    private const int StartRoundDelayMs = 2 * 1000;
    private const int ShowResultDurationMs = 3 * 1000;
    private const int RoundTimeMs = 5 * 1000;

    #endregion

    public GameInstance(
        GameService gameService,
        IServiceProvider serviceProvider,
        IClientCallService clientCallService,
        Guid guid,
        GameUser u1,
        GameUser u2)
    {
        this.Guid = guid;
        this.gameService = gameService;

        gameState = new GameState(u1, u2);
        gameBroadcaster = new GameBroadcaster(clientCallService, gameState);
        gameDataService = new GameDataService(serviceProvider);
        gameTimerService = new GameTimerService();
    }

    public async void GameStart()
    {
        Console.WriteLine($"[GameInstance | {Guid}] Game started.");

        var questions = await gameDataService.PrepareQuestions(RoundCount);
        gameState.SetQuestions(questions);

        foreach (var user in GameUsers)
        {
            var opponent = GameUsers.First(x => x.UserId != user.UserId);
            var opponentProfile = await gameDataService.GetUserProfile(opponent);

            var sendBody = new MatchStartDto
            {
                GameGuid = Guid,
                MaxRounds = gameState.MaxRounds,
                QuestionTime = RoundTimeMs,
                Opponent = new UserDataDto
                {
                    UserId = opponent.UserId,
                    UserProfile = new UserProfileDto
                    {
                        Avatar = opponentProfile!.Avatar,
                        DisplayName = opponentProfile.DisplayName
                    }
                }
            };

            Console.WriteLine($"[GameInstance | {Guid}] sending match start to '{user.UserId}'.");
            gameBroadcaster.SendMatchStart(sendBody, user.UserId);
        }
    }

    public void UserReady(long userId)
    {
        lock (gameLock)
        {
            gameState.GameUsersDict[userId].SetReady(true);
            Console.WriteLine($"[GameInstance | {Guid}] User with Id '{userId}' is ready.");

            var opponent = GameUsers.First(x => x.UserId != userId);
            if (opponent.IsReady)
            {
                Console.WriteLine($"[GameInstance | {Guid}] All users are ready. Starting rounds ...");
                gameTimerService.ScheduleJobAsync(StartRoundDelayMs, RoundStart);
            }
        }
    }

    private Task RoundStart()
    {
        Console.WriteLine($"[GameInstance | {Guid}] Round {gameState.RoundNumber} started.");

        foreach (var user in GameUsers)
            user.SetReady(false);

        var question = gameState.Questions![gameState.RoundIndex];

        var data = new RoundStartDto
        {
            RoundNumber = gameState.RoundNumber,
            Question = new QuestionDto
            {
                Category = question.Category.Title,
                Title = question.Title,
                Answers = question.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    Title = a.Title
                }).ToList()
            },
        };

        gameBroadcaster.SendRoundStart(data);
        gameTimerService.ScheduleJobAsync(RoundTimeMs, RoundResult);
        return Task.CompletedTask;
    }

    public void UserAnswer(long userId, long answerId)
    {
        lock (gameLock)
        {
            if (gameState.GameUsersDict[userId].IsAnswered)
                return;

            gameState.GameUsersDict[userId].SetAnswer(answerId);

            Console.WriteLine(
                $"[GameInstance | {Guid}] User with Id '{userId}' answered with id '{answerId}'.");

            var opponent = GameUsers.First(x => x.UserId != userId);
            if (opponent.IsAnswered)
            {
                Console.WriteLine($"[GameInstance | {Guid}] All users are answered.");
                RoundResult();
            }
        }
    }

    private Task RoundResult()
    {
        Console.WriteLine($"[GameInstance | {Guid}] Round {gameState.RoundNumber} result.");

        //Calculating answers
        var question = gameState.Questions![gameState.RoundIndex];
        var correctAnswerId = question.Answers.Single(a => a.IsCorrect).Id;
        foreach (var user in GameUsers)
        {
            if (user.IsAnswered && user.AnswerId == correctAnswerId)
                user.AddScore();
        }

        //Create data body
        var data = new RoundResultDto
        {
            CorrectAnswerId = correctAnswerId,
            UsersData = GameUsers.Select(u => new UserRoundResultDto
                {
                    UserId = u.UserId,
                    AnswerId = u.AnswerId,
                    Score = u.Score
                })
                .ToList()
        };

        //Send Data
        gameBroadcaster.SendRoundResult(data);

        //Clear Round Data
        foreach (var user in GameUsers)
            user.SetAnswer(null);

        //Finish Round
        gameTimerService.ScheduleJobAsync(ShowResultDurationMs, RoundFinish);

        return Task.CompletedTask;
    }

    private Task RoundFinish()
    {
        Console.WriteLine($"[GameInstance | {Guid}] round {gameState.RoundNumber} finished.");

        gameState.RoundIndex++;

        if (gameState.RoundIndex >= gameState.MaxRounds)
            GameFinish();
        else gameBroadcaster.RequestGetReady();

        return Task.CompletedTask;
    }

    public void RemoveUser(long userId)
    {
        gameState.GameUsersDict.Remove(userId);
        Console.WriteLine($"[GameInstance | {Guid}] User {userId} removed.");
        GameFinish();
    }

    private async void GameFinish()
    {
        foreach (var user in GameUsers)
        {
            var opponent = GameUsers.FirstOrDefault(x => x.UserId != user.UserId);
            var matchResultDto = new MatchResultDto
            {
                MatchState = opponent == null
                    ? MatchResultDto.State.Win // Opponent Left the game
                    : (
                        user.Score == opponent.Score ? MatchResultDto.State.Draw :
                        user.Score > opponent.Score ? MatchResultDto.State.Win : MatchResultDto.State.Lose
                    ),
                Score = user.Score,
                OpponentLeft = opponent == null
            };

            if (user is not BotGameUser && user.Score > 0)
                await gameDataService.AddScore(user.UserId, user.Score);
            
            gameBroadcaster.SendMatchResult(user, matchResultDto);
        }

        Console.WriteLine($"[GameInstance | {Guid}] Game finished.");
        GameClose();
    }

    public void GameClose()
    {
        gameTimerService.DisposeTimer();
        gameService.DisposeGame(this);

        Console.WriteLine($"[GameInstance | {Guid}] Game closed.");
    }
}