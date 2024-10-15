namespace Docentify.Application.Courses.Commands;

public class UpdateCourseCommand
{
    public int? Id { private get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsRequired { get; set; }
    public int? RequiredTimeLimit { get; set; }
    
    public int? GetId()
    {
        return Id;
    }
}