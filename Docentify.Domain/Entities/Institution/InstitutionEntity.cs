using System.ComponentModel.DataAnnotations;
using Docentify.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities;

[Table("institutions")]
[Index("Email", Name = "email_UNIQUE", IsUnique = true)]
[Index("Name", Name = "name_UNIQUE", IsUnique = true)]
[Index("Document", Name = "document_UNIQUE", IsUnique = true)]
public class InstitutionEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("telephone")]
    [StringLength(45)]
    public string? Telephone { get; set; }
    
    [Column("document")]
    [StringLength(45)]
    public string Document { get; set; } = null!;
    
    [Column("address")]
    [StringLength(350)]
    public string? Address { get; set; }

    // [InverseProperty("Institution")]
    // public virtual ICollection<CourseEntity> Courses { get; set; } = new List<CourseEntity>();

    [InverseProperty("Institution")]
    public virtual InstitutionPasswordHashEntity? InstitutionPasswordHash { get; set; }

    [ForeignKey("InstitutionId")]
    [InverseProperty("Institutions")]
    public virtual ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
}
