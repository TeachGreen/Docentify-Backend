namespace Docentify.Application.Common.Queries;

public class PagedQuery
{
    public int Page { get; set; } = 1;
    public int Amount { get; set; } = 100;
}