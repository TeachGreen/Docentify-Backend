using Docentify.Application.Common.Queries;

namespace Docentify.Application.Courses.Queries;

public class GetInstitutionCoursesQuery : PagedQuery
{
    public int? InstitutionId { get; set; }
    public string? Name { get; set; }
    public bool? IsRequired { get; set; }
}