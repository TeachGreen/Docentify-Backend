using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Step;

[PrimaryKey("Id")]
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
    public bool? IsCorrect { get; set; }

    [Column("questionId")]
    public int QuestionId { get; set; }
    
    [ForeignKey("QuestionId")]
    [InverseProperty("Options")]
    public virtual QuestionEntity Question { get; set; } = null!;
}
