using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Step;

[PrimaryKey("Id", "QuestionId")]
[Table("options")]
[Index("QuestionId", Name = "questionId")]
public class OptionEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("text", TypeName = "text")]
    public string Text { get; set; } = null!;

    [Column("isCorrect")]
    public sbyte? IsCorrect { get; set; }

    [Key]
    [Column("questionId")]
    public int QuestionId { get; set; }
}
