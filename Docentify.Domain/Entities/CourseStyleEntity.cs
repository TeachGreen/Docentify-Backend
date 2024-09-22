namespace Docentify.Domain.Entities;

public class CourseStyleEntity : BaseEntity
{
    public string? Name { get; set; }

    public int CourseId { get; set; }

    public virtual CourseEntity Course { get; set; } = null!;

    public virtual ICollection<StyleVariablesValueEntity> Stylevariablesvalues { get; set; } = new List<StyleVariablesValueEntity>();
}
