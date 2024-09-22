namespace Docentify.Domain.Entities;

public class QuestionEntity : BaseEntity
{
    public string Statement { get; set; } = null!;

    public int ActivityId { get; set; }
}
