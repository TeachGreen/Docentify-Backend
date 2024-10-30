using Docentify.Application.Activities.ValueObjects;

namespace Docentify.Application.Activities.Commands;

public class InsertQuestionCommand
{
    public string? Statement { get; set; } = null!;
    public int? ActivityId { get; set; }
    public List<OptionValueObject>? Options { get; set; }
}