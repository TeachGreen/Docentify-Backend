using Docentify.Domain.Enums;

namespace Docentify.Application.Steps.Commands;

public class UpdateStepCommand
{
    public int? StepId { get; set; }
    public string? Order { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
}