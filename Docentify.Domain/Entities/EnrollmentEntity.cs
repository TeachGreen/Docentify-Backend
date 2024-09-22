namespace Docentify.Domain.Entities;

public class EnrollmentEntity : BaseEntity
{
    public DateTime? EnrollmentDate { get; set; }

    public sbyte? IsRequired { get; set; }

    public DateTime? RequiredDate { get; set; }

    public int UserId { get; set; }

    public int CourseId { get; set; }

    public virtual CourseEntity Course { get; set; } = null!;

    public virtual UserEntity User { get; set; } = null!;

    public virtual ICollection<UserProgressEntity> UserProgresses { get; set; } = new List<UserProgressEntity>();
}
