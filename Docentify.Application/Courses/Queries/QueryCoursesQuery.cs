using Docentify.Application.Common.Queries;
using Docentify.Domain.Enums;

namespace Docentify.Application.Courses.Queries;

public class QueryCoursesQuery : PagedQuery
{
    public string? Name { get; set; }
    public bool? IsRequired { get; set; }
    public bool? OnlyFavorites { get; set; } = false;
    public ECourseProgress? Progress { get; set; }
    public string OrderBy { get; set; } = "Name";
    public bool OrderByDescending { get; set; } = false;
}