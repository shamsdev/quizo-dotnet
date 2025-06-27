using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KarizmaPlatform.Core.Database;

namespace QuizoDotnet.Domain.Models.Users;

[Table("user_profiles")]
public class UserProfile : BaseEntity
{
    [Column("user_id"), Required] public long UserId { get; init; }
    [Column("avatar"), MaxLength(20)] public string Avatar { get; set; }

    [Column("display_name"), MaxLength(20)]
    public string? DisplayName { get; set; }

    public User? User { get; set; }
}