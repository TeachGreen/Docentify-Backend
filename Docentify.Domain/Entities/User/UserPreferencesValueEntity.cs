using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.User;

[PrimaryKey("UserId", "PreferenceId")]
[Table("userpreferencesvalues")]
[Index("PreferenceId", Name = "fk_UserPreferencesValues_UserPreferences1")]
public partial class UserPreferencesValueEntity
{
    [Key]
    [Column("userId")]
    public int UserId { get; set; }

    [Key]
    [Column("preferenceId")]
    public int PreferenceId { get; set; }

    [Column("value")]
    [StringLength(45)]
    public string Value { get; set; } = null!;

    [ForeignKey("PreferenceId")]
    [InverseProperty("UserPreferencesValues")]
    public virtual UserPreferenceEntity Preference { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserPreferencesValues")]
    public virtual UserEntity User { get; set; } = null!;
}
