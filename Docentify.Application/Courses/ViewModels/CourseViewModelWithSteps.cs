using Docentify.Application.Courses.ValueObjects;

namespace Docentify.Application.Courses.ViewModels;

public class CourseViewModelWithSteps
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public List<StepValueObject> Steps;
}