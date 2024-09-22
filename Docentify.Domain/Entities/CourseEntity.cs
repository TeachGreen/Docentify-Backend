namespace Docentify.Domain.Entities;

public class CourseEntity : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int InstitutionId { get; set; }

    public virtual ICollection<CourseStyleEntity> CourseStyles { get; set; } = new List<CourseStyleEntity>();

    public virtual ICollection<EnrollmentEntity> Enrollments { get; set; } = new List<EnrollmentEntity>();
    
    public virtual InstitutionEntity Institution { get; set; } = null!;

    public virtual ICollection<StepEntity> Steps { get; set; } = new List<StepEntity>();
}
