namespace Docentify.Application.Activities.ValueObjects;

public class QuestionValueObject
{
    public int Id { get; set; }
    public string Statement { get; set; } = null!;
    public List<OptionValueObject> Options { get; set; }
}