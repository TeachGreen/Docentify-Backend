namespace Docentify.Domain.Entities;

public class UserPreferencesValueEntity
{
    public int UserId { get; set; }

    public int PreferenceId { get; set; }

    public string Value { get; set; } = null!;

    public virtual UserPreferenceEntity Preference { get; set; } = null!;

    public virtual UserEntity User { get; set; } = null!;
}
