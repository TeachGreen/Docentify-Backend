using Docentify.Application.Courses.ValueObjects;

namespace Docentify.Application.Courses.ViewModels;

public class CourseWithStepsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Duration { get; set; }
    public bool IsRequired { get; set; }
    public bool IsEnrolled { get; set; }
    public DateTime? RequiredDate { get; set; }
    public string Image { get; set; }
    public List<StepValueObject> Steps;
}