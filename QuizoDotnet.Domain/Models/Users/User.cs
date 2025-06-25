using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KarizmaPlatform.Core.Database;

namespace QuizoDotnet.Domain.Models.Users;

[Table("users")]
public class User : BaseEntity
{
    [Column("password"), Required, MaxLength(64)]
    public required string Password { get; init; }

    [Column("last_login_at")] public DateTimeOffset? LastLoginDate { get; private set; }

    public UserProfile? UserProfile { get; set; }

    public void UpdateLastLogin()
    {
        LastLoginDate = DateTimeOffset.UtcNow;
    }
}