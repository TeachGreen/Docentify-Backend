using Docentify.Application.Activities.ValueObjects;

namespace Docentify.Application.Activities.ViewModels;

public class ActivityViewModel
{
    public int Id { get; set; }
    public int AllowedAttempts { get; set; }
    public int StepId { get; set; }
    public List<QuestionValueObject> Questions { get; set; }
}