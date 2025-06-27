using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizoDotnet.Application.Services;

namespace QuizoDotnet.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddSingleton<TokenService>()
            .AddScoped<UserService>()
            ;

        return services;
    }
}