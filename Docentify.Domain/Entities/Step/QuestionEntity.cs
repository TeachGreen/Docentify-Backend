using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Step;

[PrimaryKey("Id", "ActivityId")]
[Table("questions")]
[Index("ActivityId", Name = "activityId")]
public class QuestionEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("statement", TypeName = "text")]
    public string Statement { get; set; } = null!;

    [Key]
    [Column("activityId")]
    public int ActivityId { get; set; }
}
