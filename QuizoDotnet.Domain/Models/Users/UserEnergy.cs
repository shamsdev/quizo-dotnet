using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KarizmaPlatform.Core.Database;

namespace QuizoDotnet.Domain.Models.Users;

[Table("user_energies")]
public class UserEnergy : BaseEntity
{
    public const int MAX_ENERGY = 5;
    public const int SECONDS_PER_ENERGY = 300;

    [Column("user_id"), Required] public long UserId { get; init; }

    [Column("amount"), Required] public int Amount { get; set; } = 0;

    [Column("last_energy_updated_at"), Required]
    public DateTimeOffset LastEnergyUpdatedAt { get; set; }

    public DateTimeOffset NextGenerationAt => LastEnergyUpdatedAt.AddSeconds(SECONDS_PER_ENERGY);

    public User User { get; set; } = null!;
}