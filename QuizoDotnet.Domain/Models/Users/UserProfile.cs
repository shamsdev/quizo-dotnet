using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KarizmaPlatform.Core.Database;

namespace QuizoDotnet.Domain.Models.Users;

[Table("user_profiles")]
public class UserProfile(long userId, string avatar, string? displayName = null) : BaseEntity
{
    [Column("user_id"), Required] public long UserId { get; set; } = userId;
    [Column("avatar"), MaxLength(20)] public string? Avatar { get; set; } = avatar;

    [Column("display_name"), MaxLength(20)]
    public string? DisplayName { get; set; } = displayName;

    public User? User { get; set; }
}