namespace Docentify.Application.Activities.ValueObjects;

public class OptionValueObject
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public bool? IsCorrect { get; set; }
}