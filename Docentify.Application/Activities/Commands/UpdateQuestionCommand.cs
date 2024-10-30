using Docentify.Application.Activities.ValueObjects;

namespace Docentify.Application.Activities.Commands;

public class UpdateQuestionCommand
{
    public int? QuestionId { get; set; }
    public string? Statement { get; set; } = null!;
    public List<OptionValueObject>? Options { get; set; }
}