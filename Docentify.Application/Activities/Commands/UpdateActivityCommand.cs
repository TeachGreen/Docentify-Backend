namespace Docentify.Application.Activities.Commands;

public class UpdateActivityCommand
{
    public int? ActivityId { get; set; }
    public int? AllowedAttempts { get; set; }
}