namespace Docentify.Domain.Entities;

public class UserProgressEntity
{
    public int EnrollmentId { get; set; }

    public int StepId { get; set; }

    public DateTime ProgressDate { get; set; }

    public virtual EnrollmentEntity Enrollment { get; set; } = null!;

    public virtual StepEntity Step { get; set; } = null!;
}
