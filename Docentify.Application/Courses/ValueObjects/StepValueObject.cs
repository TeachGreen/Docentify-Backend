using Docentify.Domain.Enums;

namespace Docentify.Application.Courses.ValueObjects;

public class StepValueObject
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public EStepType Type { get; set; }
    public bool IsCompleted { get; set; }
}