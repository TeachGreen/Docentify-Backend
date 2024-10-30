namespace Docentify.Application.Activities.Commands;

public class InsertActivityCommand
{
    public int? AllowedAttempts { get; set; }
    public int? StepId { get; set; }
}