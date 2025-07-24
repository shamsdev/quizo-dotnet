using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KarizmaPlatform.Core.Database;

namespace QuizoDotnet.Domain.Models.Users;

[Table("user_scores")]
public class UserScore : BaseEntity
{
    [Column("user_id"), Required] public long UserId { get; init; }
    [Column("score"), Required] public int Score { get; set; }

    public User User { get; set; } = null!;
}