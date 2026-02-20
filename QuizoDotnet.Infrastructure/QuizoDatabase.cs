using KarizmaPlatform.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuizoDotnet.Domain.Models.Questions;
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
    public DbSet<UserScore> UserScores { get; init; }
    public DbSet<Category> Categories { get; init; }
    public DbSet<Question> Questions { get; init; }
    public DbSet<QuestionAnswer> QuestionAnswers { get; init; }

    public DbSet<UserResource> UserResources { get; init; }
    public DbSet<UserEnergy> UserEnergies { get; init; }

    public QuizoDatabase() : this(new DbContextOptionsBuilder<QuizoDatabase>()
        .UseNpgsql(PostgresConnectionString).Options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne(qa => qa.Question)
            .HasForeignKey(qa => qa.QuestionId);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Category)
            .WithMany()
            .HasForeignKey(q => q.CategoryId);
    }
}