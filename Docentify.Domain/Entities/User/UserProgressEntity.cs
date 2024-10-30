using System.ComponentModel.DataAnnotations;
using Docentify.Domain.Entities.Courses;
using Docentify.Domain.Entities.Step;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.User;

[PrimaryKey("EnrollmentId", "StepId")]
[Table("userprogress")]
[Index("StepId", Name = "stepId")]
public class UserProgressEntity
{
    [Key]
    [Column("enrollmentId")]
    public int EnrollmentId { get; set; }

    [Key]
    [Column("stepId")]
    public int StepId { get; set; }

    [Column("progressDate", TypeName = "datetime")]
    public DateTime ProgressDate { get; set; }

    [ForeignKey("EnrollmentId")]
    [InverseProperty("UserProgresses")]
    public virtual EnrollmentEntity Enrollment { get; set; } = null!;

    [ForeignKey("StepId")]
    [InverseProperty("UserProgresses")]
    public virtual StepEntity Step { get; set; } = null!;
}
