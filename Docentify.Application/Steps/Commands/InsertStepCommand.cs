using Docentify.Domain.Enums;

namespace Docentify.Application.Steps.Commands;

public class InsertStepCommand
{
    public string? Order { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public EStepType? Type { get; set; }
    public string? Content { get; set; }
    public int? CourseId { get; set; }
}