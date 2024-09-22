namespace Docentify.Domain.Entities;

public class StyleVariablesValueEntity
{
    public int StyleId { get; set; }

    public int VariableId { get; set; }

    public string Value { get; set; } = null!;

    public virtual CourseStyleEntity Style { get; set; } = null!;

    public virtual StyleVariableEntity Variable { get; set; } = null!;
}
