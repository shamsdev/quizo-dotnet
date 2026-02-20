using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KarizmaPlatform.Core.Database;

namespace QuizoDotnet.Domain.Models.Users;

[Table("user_resources")]
public class UserResource : BaseEntity
{
    [Column("user_id"), Required] public long UserId { get; init; }

    [Column("xp"), Required] public int Xp { get; set; } = 0;
    [Column("coin"), Required] public int Coin { get; set; } = 0;

    public User User { get; set; } = null!;
}