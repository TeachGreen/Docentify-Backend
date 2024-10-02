namespace Docentify.Domain.Entities;

public class InstitutionEntity : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telephone { get; set; }
    
    public virtual InstitutionPasswordHashEntity InstitutionPasswordHash { get; set; }

    public virtual ICollection<CourseEntity> Courses { get; set; } = new List<CourseEntity>();

    public virtual ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
}
