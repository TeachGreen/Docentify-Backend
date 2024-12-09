namespace Docentify.Application.Ranking.ValueObjects;

public class RankingPositionValueObject
{
    public int UserId { get; set; }
    public int Position { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
}