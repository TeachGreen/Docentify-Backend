namespace Docentify.Application.Courses.ViewModels;

public class CourseViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool? IsRequired { get; set; }
    public bool? IsFavorited { get; set; }
    public bool? IsEnrolled { get; set; }
    public int? RequiredTimeLimit { get; set; }
    public int? Duration { get; set; }
    public string? Image { get; set; }
}