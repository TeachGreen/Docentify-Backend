namespace Docentify.Domain.Entities;

public class FileStepEntity : BaseEntity
{
    public byte[] Data { get; set; } = null!;

    public int StepId { get; set; }

    public virtual StepEntity Step { get; set; } = null!;
}
