using System.ComponentModel.DataAnnotations;

namespace Docentify.Domain.Entities.User;

[Table("userpreferences")]
public class UserPreferenceEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("defaultValue")]
    [StringLength(45)]
    public string DefaultValue { get; set; } = null!;

    [InverseProperty("Preference")]
    public virtual ICollection<UserPreferencesValueEntity> UserPreferencesValues { get; set; } = new List<UserPreferencesValueEntity>();
}
