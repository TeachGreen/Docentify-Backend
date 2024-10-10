namespace Docentify.Domain.Entities;

public class UserPreferenceEntity : BaseEntity
{
    public string Name { get; set; } = null!;
    public string DefaultValue { get; set; } = null!;
}
