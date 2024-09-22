namespace Docentify.Domain.Entities;

public class VideoStepEntity : BaseEntity
{
    public string Url { get; set; } = null!;

    public int StepId { get; set; }

    public virtual StepEntity Step { get; set; } = null!;
}
