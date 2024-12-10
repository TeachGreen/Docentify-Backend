using Docentify.Application.Ranking.ValueObjects;

namespace Docentify.Application.Ranking.ViewModels;

public class RankingViewModel
{
    public List<RankingPositionValueObject> Rankings { get; set; }
    public int MaxPage { get; set; }
    public int UserRanking { get; set; }
}