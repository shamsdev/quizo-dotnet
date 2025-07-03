using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KarizmaPlatform.Core.Database;

namespace QuizoDotnet.Domain.Models.Questions;

[Table("categories")]
public class Category : BaseEntity
{
    [Column("title"), Required, MaxLength(255)]
    public required string Title { get; set; }
}