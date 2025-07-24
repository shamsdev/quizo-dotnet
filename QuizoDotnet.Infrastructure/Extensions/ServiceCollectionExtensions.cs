using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using QuizoDotnet.Application.Interfaces.Repositories.Users;
using QuizoDotnet.Application.Interfaces.Repositories.Game;
using QuizoDotnet.Infrastructure.Repositories;
using QuizoDotnet.Infrastructure.Repositories.Game;

namespace QuizoDotnet.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceBuilder =
            new NpgsqlDataSourceBuilder(configuration.GetConnectionString("PostgresConnectionString"));
        var dataSource = dataSourceBuilder.Build();
        services.AddSingleton(dataSource);
        services.AddDbContextPool<QuizoDatabase>(options => options.UseNpgsql(dataSource));
        return services.AddRepositories();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUserProfileRepository, UserProfileRepository>()
            .AddScoped<IUserScoreRepository, UserScoreRepository>()
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<IQuestionRepository, QuestionRepository>()
            .AddScoped<IQuestionAnswerRepository, QuestionAnswerRepository>()
            ;

        return services;
    }
}