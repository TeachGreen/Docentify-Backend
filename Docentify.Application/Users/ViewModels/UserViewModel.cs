namespace Docentify.Application.Users.ViewModels;

public class UserViewModel
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
    public string? Telephone { get; set; }
    public string Document { get; set; }
    public string? Gender { get; set; }
    public int CompletedCourses { get; set; }
    public int OngoingCourses { get; set; }
    public int CancelledEnrollments { get; set; }
}