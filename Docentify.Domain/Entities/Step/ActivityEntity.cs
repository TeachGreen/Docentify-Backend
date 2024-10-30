using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Step;

[PrimaryKey("Id")]
[Table("activities")]
[Index("StepId", Name = "stepId")]
public class ActivityEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Key]
    [Column("allowedAttempts")]
    public int AllowedAttempts { get; set; }

    [Key]
    [Column("stepId")]
    public int StepId { get; set; }

    [ForeignKey("StepId")]
    [InverseProperty("Activity")]
    public virtual StepEntity Step { get; set; } = null!;
    
    [InverseProperty("Activity")]
    public virtual ICollection<QuestionEntity> Questions { get; set; } = new List<QuestionEntity>();
    
    [InverseProperty("Activity")]
    public virtual ICollection<AttemptEntity> Attempts { get; set; } = new List<AttemptEntity>();
}
