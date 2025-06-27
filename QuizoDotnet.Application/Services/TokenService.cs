using System.Text.Json;
using Microsoft.Extensions.Configuration;
using QuizoDotnet.Application.Utils;

namespace QuizoDotnet.Application.Services;

public class TokenService(IConfiguration configuration)
{
    private string AccessTokenKey => configuration["AccessTokenKey"]!;

    public record AccessTokenData
    {
        public long UserId { get; init; }
        public string Password { get; init; } = null!;
    };

    public string GetAccessToken(AccessTokenData accessTokenData)
    {
        var accessTokenDataJson = JsonSerializer.SerializeToNode(accessTokenData)!
            .ToJsonString()
            .Trim()
            .Replace(" ", string.Empty);

        return CryptUtils.Encrypt(accessTokenDataJson, AccessTokenKey);
    }

    public AccessTokenData GetAccessTokenData(string accessToken)
    {
        var accessTokenDataJson = CryptUtils.Decrypt(accessToken, AccessTokenKey);
        return JsonSerializer.Deserialize<AccessTokenData>(accessTokenDataJson)!;
    }
}