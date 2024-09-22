namespace Docentify.Domain.Entities;

public class OptionEntity : BaseEntity
{
    public string Text { get; set; } = null!;

    public sbyte? IsCorrect { get; set; }

    public int QuestionId { get; set; }
}
