namespace Docentify.Domain.Entities;

public class UserPreferenceEntity : BaseEntity
{
    public string PreferenceName { get; set; } = null!;

    public string DefaultValue { get; set; } = null!;

    public virtual ICollection<UserPreferencesValueEntity> Userpreferencesvalues { get; set; } = new List<UserPreferencesValueEntity>();
}
