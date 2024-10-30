using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Step;

[PrimaryKey("Id")]
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
    
    [ForeignKey("ActivityId")]
    [InverseProperty("Questions")]
    public virtual ActivityEntity Activity { get; set; } = null!;

    [InverseProperty("Question")]
    public virtual ICollection<OptionEntity> Options { get; set; } = new List<OptionEntity>();
}
