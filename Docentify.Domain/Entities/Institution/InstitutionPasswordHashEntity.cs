using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities;

[Table("institutionpasswordhashes")]
[Index("InstitutionId", Name = "institutionId")]
public class InstitutionPasswordHashEntity : PasswordHash
{
    [Column("institutionId")]
    public int InstitutionId { get; set; }

    [ForeignKey("InstitutionId")]
    [InverseProperty("InstitutionPasswordHash")]
    public virtual InstitutionEntity Institution { get; set; } = null!;
}
