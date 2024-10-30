namespace Docentify.Application.Activities.ViewModels;

public class AttemptViewModel
{
    public int UserId { get; set; }
    public int ActivityId { get; set; }
    public int Score { get; set; }
    public DateTime Date { get; set; }
    public bool Passed { get; set; }
}