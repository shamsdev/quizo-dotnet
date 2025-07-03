using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KarizmaPlatform.Core.Database;

namespace QuizoDotnet.Domain.Models.Questions;

[Table("question_answers")]
public class QuestionAnswer : BaseEntity
{
    [Column("question_id"), Required] public long QuestionId { get; set; }

    [ForeignKey(nameof(QuestionId))] public Question Question { get; set; } = null!;

    [Column("title"), Required, MaxLength(255)]
    public required string Title { get; set; }

    [Column("is_correct")] public bool IsCorrect { get; set; }
}