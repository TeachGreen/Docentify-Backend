using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Step;

[PrimaryKey("Id", "StepId")]
[Table("activities")]
[Index("StepId", Name = "stepId")]
public class ActivityEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Key]
    [Column("stepId")]
    public int StepId { get; set; }

    [ForeignKey("StepId")]
    [InverseProperty("Activities")]
    public virtual StepEntity Step { get; set; } = null!;
}
