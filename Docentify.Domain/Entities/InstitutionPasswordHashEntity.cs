namespace Docentify.Domain.Entities;

public class InstitutionPasswordHashEntity : BaseEntity
{
    public string HashedPassword { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public int InstitutionId { get; set; }

    public virtual InstitutionEntity Institution { get; set; } = null!;
}
