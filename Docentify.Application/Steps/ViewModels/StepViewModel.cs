using Docentify.Domain.Enums;

namespace Docentify.Application.Steps.ViewModels;

public class StepViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public EStepType Type { get; set; } 
    public string Content { get; set; }
    public bool IsCompleted { get; set; }
    public int? AssociatedActivity { get; set; }
}