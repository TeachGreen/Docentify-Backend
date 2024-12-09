namespace Docentify.Application.Ranking.Queries;

public class GetRankingQuery
{
    public int? Page { get; set; } = 1;
    
    public string? Search { get; set; }
}
