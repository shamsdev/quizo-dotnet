using KarizmaPlatform.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Infrastructure;

public class QuizoDatabase(DbContextOptions options) : BaseContext(options)
{
    private static string? PostgresConnectionString => new ConfigurationBuilder()
        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../QuizoDotnet/"))
        .AddJsonFile("appsettings.json", false)
        .Build()
        .GetConnectionString("PostgresConnectionString");

    public DbSet<User> Users { get; init; }
    public DbSet<UserProfile> UserProfiles { get; init; }

    public QuizoDatabase() : this(new DbContextOptionsBuilder<QuizoDatabase>()
        .UseNpgsql(PostgresConnectionString).Options)
    {
    }
}