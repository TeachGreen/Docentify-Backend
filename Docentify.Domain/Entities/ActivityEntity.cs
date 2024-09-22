namespace Docentify.Domain.Entities;

public class ActivityEntity : BaseEntity
{
    public int StepId { get; set; }

    public virtual StepEntity Step { get; set; } = null!;
}
