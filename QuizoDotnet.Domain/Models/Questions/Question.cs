using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KarizmaPlatform.Core.Database;

namespace QuizoDotnet.Domain.Models.Questions;

[Table("questions")]
public class Question : BaseEntity
{
    [Column("title"), Required, MaxLength(255)]
    public required string Title { get; set; }

    [Column("category_id"), Required] public long CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))] public Category Category { get; set; } = null!;

    public ICollection<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();
}