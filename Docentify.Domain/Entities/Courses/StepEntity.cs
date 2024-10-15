using System.ComponentModel.DataAnnotations;
using Docentify.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Courses;

[Table("steps")]
[Index("CourseId", Name = "courseId")]
public class StepEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("order")]
    public int Order { get; set; }

    [Column("description")]
    [StringLength(45)]
    public string Description { get; set; } = null!;

    [Column("type")]
    public int Type { get; set; }

    [Column("courseId")]
    public int CourseId { get; set; }

    // [InverseProperty("Step")]
    // public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    [ForeignKey("CourseId")]
    [InverseProperty("Steps")]
    public virtual CourseEntity Course { get; set; } = null!;

    // [InverseProperty("Step")]
    // public virtual ICollection<Filestep> Filesteps { get; set; } = new List<Filestep>();
    
    [InverseProperty("Step")]
    public virtual ICollection<UserProgressEntity> UserProgresses { get; set; } = new List<UserProgressEntity>();

    // [InverseProperty("Step")]
    // public virtual ICollection<Videostep> Videosteps { get; set; } = new List<Videostep>();
}
