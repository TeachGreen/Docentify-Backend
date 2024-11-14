namespace Docentify.Application.Courses.Commands;

public class InsertCourseCommand
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsRequired { get; set; }
    public int? RequiredTimeLimit { get; set; }
    public int? Duration { get; set; }
}