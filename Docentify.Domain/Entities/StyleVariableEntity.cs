namespace Docentify.Domain.Entities;

public class StyleVariableEntity : BaseEntity
{

    public string Name { get; set; } = null!;

    public string? DefaultValue { get; set; }

    public virtual ICollection<StyleVariablesValueEntity> Stylevariablesvalues { get; set; } = new List<StyleVariablesValueEntity>();
}
