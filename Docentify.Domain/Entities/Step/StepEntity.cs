using System.ComponentModel.DataAnnotations;
using Docentify.Domain.Entities.Courses;
using Docentify.Domain.Entities.User;
using Docentify.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Step;

[Table("steps")]
[Index("CourseId", Name = "courseId")]
public class StepEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("order")]
    public int Order { get; set; }

    [Column("title")]
    [StringLength(500)]
    public string Title { get; set; } = null!;
    
    [Column("description")]
    [StringLength(45)]
    public string Description { get; set; } = null!;

    [Column("type")]
    public EStepType Type { get; set; }
    
    [Column("Content")]
    public string Content { get; set; } = null!;

    [Column("courseId")]
    public int CourseId { get; set; }

    [InverseProperty("Step")]
    public virtual ICollection<ActivityEntity> Activities { get; set; } = new List<ActivityEntity>();

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
