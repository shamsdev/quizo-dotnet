using Microsoft.Extensions.DependencyInjection;
using QuizoDotnet.Application.Interfaces;
using QuizoDotnet.Application.Interfaces.Repositories.Game;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Domain.Models.Questions;
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
    private int roundIndex = 0;
    public int RoundNumber => roundIndex + 1;

    private List<Question>? questions;
    private int MaxRounds => questions!.Count;

    private CancellationTokenSource tasksCancellationTokenSource = new();

    #region Delays and Times

    private const int StartRoundDelay = 2 * 1000;
    private const int ShowResultsDelay = 5 * 1000;
    private const int QuestionTime = 5 * 1000;

    #endregion

    #region Commands

    private const string MatchStartCommand = "match/start";
    private const string GetReadyCommand = "match/get-ready";
    private const string StartRoundCommand = "match/start-round";
    private const string RoundResultCommand = "match/round-result";

    #endregion

    private readonly Dictionary<long, GameUserInstance> gameUsers = new()
    {
        { u1.UserId, u1 },
        { u2.UserId, u2 }
    };

    public async void Start()
    {
        Console.WriteLine($"[GameInstance | {guid}] Match start called.");

        await PrepareQuestions();

        foreach (var user in gameUsers.Values)
        {
            var opponent = gameUsers.Values.First(x => x.UserId != user.UserId);
            var opponentProfile = await GetUserProfile(opponent.UserId);

            var sendBody = new
            {
                GameGuid = guid,
                MaxRounds,
                QuestionTime,
                Opponent = new
                {
                    UserId = opponent.UserId,
                    Avatar = opponentProfile!.Avatar,
                    DisplayName = opponentProfile.DisplayName
                }
            };

            Console.WriteLine($"[GameInstance | {guid}] sending match start to '{user.UserId}'.");
            await clientCallService.Send(user.ConnectionId, MatchStartCommand, sendBody);
        }

        //TODO should implement a timeout for players ready and start round
    }

    private void SendAll(string address, object body)
    {
        foreach (var user in gameUsers.Values)
            clientCallService.Send(user.ConnectionId, address, body);
    }

    private async Task<UserProfile?> GetUserProfile(long userId)
    {
        Console.WriteLine($"[GameInstance | {guid}] Getting user profile '{userId}'.");
        using var scope = serviceProvider.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        return await userProfileRepository.GetByUserId(userId);
    }

    private async Task PrepareQuestions()
    {
        Console.WriteLine($"[GameInstance | {guid}] Preparing game questions.");
        using var scope = serviceProvider.CreateScope();
        var questionRepository = scope.ServiceProvider.GetRequiredService<IQuestionRepository>();
        questions = await questionRepository.GetRandomQuestions(5);
    }

    public void RemoveUser(long userId)
    {
        gameUsers.Remove(userId);
        ForceClose();
    }

    public void UserReady(long userId)
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

    public void UserAnswer(long userId, long answerId)
    {
        lock (gameLock)
        {
            gameUsers[userId].SetAnswer(answerId);

            Console.WriteLine($"[GameInstance | {guid}] User with Id '{userId}' answered with id '{answerId}'.");
            var opponent = gameUsers.Values.First(x => x.UserId != userId);
            if (opponent.IsAnswered)
            {
                Console.WriteLine($"[GameInstance | {guid}] All users are answered. Showing result ...");
                tasksCancellationTokenSource.Cancel();
            }
        }
    }

    private void RequestGetReady()
    {
        SendAll(GetReadyCommand, new { RoundNumber });
    }

    private async void StartRound()
    {
        foreach (var user in gameUsers.Values)
            user.SetReady(false);

        try
        {
            await Task.Delay(StartRoundDelay, tasksCancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            return;
        }

        var question = questions![roundIndex];
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
            RoundNumber
        };

        SendAll(StartRoundCommand, data);
        RoundTimer();
    }

    private async void RoundTimer()
    {
        Console.WriteLine($"[GameInstance | Round timer started.");
        try
        {
            await Task.Delay(QuestionTime, tasksCancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            return;
        }

        RoundResult();
    }

    private async void RoundResult()
    {
        Console.WriteLine($"[GameInstance | Bell Ringed! round {roundIndex} finished.");

        //Calculating answers
        var question = questions![roundIndex];
        var correctAnswerId = question.Answers.Single(a => a.IsCorrect).Id;
        foreach (var user in gameUsers.Values)
        {
            if (user.IsAnswered && user.AnswerId == correctAnswerId)
                user.AddScore();
            user.SetAnswer(null);
        }

        //Create data body
        var data = new
        {
            AnswerId = correctAnswerId,
            UsersData = gameUsers.Values.Select(u => new
                {
                    UserId = u.UserId,
                    AnswerId = u.AnswerId,
                    Score = u.Score
                })
                .ToList()
        };

        //Send Data
        SendAll(RoundResultCommand, data);

        try
        {
            await Task.Delay(ShowResultsDelay, tasksCancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            return;
        }

        roundIndex++;
        if (roundIndex >= MaxRounds)
            GameFinish();
        else RequestGetReady();
    }

    private void GameFinish()
    {
        
        Console.WriteLine($"[GameInstance | Game finished.");
    }

    public void ForceClose(string reason = null)
    {
        tasksCancellationTokenSource.Dispose();
    }
}