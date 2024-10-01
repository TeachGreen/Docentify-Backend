namespace Docentify.Domain.Entities;

public class UserCardEntity
{
    public int UserId { get; set; }

    public int CardId { get; set; }

    public DateTime AcquirementDate { get; set; }

    public virtual CardEntity Card { get; set; } = null!;

    public virtual UserEntity User { get; set; } = null!;
}
