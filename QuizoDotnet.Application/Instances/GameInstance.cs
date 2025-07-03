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

    private CancellationTokenSource questionTimeDelaySource = new();

    #region Delays and Times

    private const int StartRoundDelay = 2 * 1000;
    private const int ShowResultsDelay = 5 * 1000;
    private const int QuestionTime = 20 * 1000;

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

        RequestGetReady();
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
                questionTimeDelaySource.Cancel();
            }
        }
    }

    private void RequestGetReady()
    {
        SendAll(GetReadyCommand, new { RoundNumber });
    }

    private async void StartRound()
    {
        await Task.Delay(StartRoundDelay);

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

        foreach (var user in gameUsers.Values)
        {
            user.SetReady(false);
            await clientCallService.Send(user.ConnectionId, StartRoundCommand, data);
        }

        RoundTimer();
    }

    private async void RoundTimer()
    {
        Console.WriteLine($"[GameInstance | Round timer started.");
        await Task.Delay(QuestionTime, questionTimeDelaySource.Token);
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

        await Task.Delay(ShowResultsDelay);

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
        throw new NotImplementedException();
    }
}