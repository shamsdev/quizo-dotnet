using KarizmaPlatform.Connection.Server.Attributes;
using KarizmaPlatform.Connection.Server.Base;
using QuizoDotnet.Application.DTOs;
using QuizoDotnet.Application.Services;

namespace QuizoDotnet.Handlers.RequestHandlers;

[RequestHandler("leaderboard")]
public class LeaderboardRequestHandler(
    UserScoreService userScoreService) : BaseRequestHandler
{
    [Action("get-top-scores")]
    public async Task<List<UserScoreDto>> GetTopScores()
    {
        var scores = await userScoreService.GetTopScores(100);
        List<UserScoreDto> userScores = [];
        userScores.AddRange(scores.Select(score => new UserScoreDto
        {
            Score = score.Score,
            UserProfile = new UserProfileDto
            {
                UserId = score.UserId,
                Avatar = score.User.UserProfile.Avatar,
                DisplayName = score.User.UserProfile.DisplayName,
            }
        }));

        return userScores;
    }
}