using System.ComponentModel.DataAnnotations;
using Docentify.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Courses;

[Table("enrollments")]
[Index("CourseId", Name = "courseId")]
[Index("UserId", Name = "userId")]
public class EnrollmentEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("enrollmentDate", TypeName = "datetime")]
    public DateTime? EnrollmentDate { get; set; }
    
    [Column("userId")]
    public int UserId { get; set; }

    [Column("courseId")]
    public int CourseId { get; set; }

    [Column("isActive")] 
    public bool IsActive { get; set; } = true;

    [ForeignKey("CourseId")]
    [InverseProperty("Enrollments")]
    public virtual CourseEntity Course { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Enrollments")]
    public virtual UserEntity User { get; set; } = null!;

    [InverseProperty("Enrollment")]
    public virtual ICollection<UserProgressEntity> UserProgresses { get; set; } = new List<UserProgressEntity>();
}
