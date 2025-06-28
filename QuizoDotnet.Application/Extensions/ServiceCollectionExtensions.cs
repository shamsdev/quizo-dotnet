using Microsoft.Extensions.DependencyInjection;
using QuizoDotnet.Application.Services;

namespace QuizoDotnet.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddSingleton<TokenService>()
            .AddScoped<UserService>()
            .AddSingleton<MatchMakeService>()
            .AddSingleton<GameService>()
            ;

        return services;
    }
}