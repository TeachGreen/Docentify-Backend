namespace Docentify.Domain.Entities;

public class UserScoreEntity
{
    public int UserId { get; set; }

    public int? Score { get; set; }

    public virtual UserEntity User { get; set; } = null!;
}
