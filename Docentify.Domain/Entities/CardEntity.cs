namespace Docentify.Domain.Entities;

public class CardEntity
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public string SilhouetteImageUrl { get; set; } = null!;

    public string AchievedImageUrl { get; set; } = null!;

    public virtual ICollection<UserCardEntity> UserCards { get; set; } = new List<UserCardEntity>();
}
