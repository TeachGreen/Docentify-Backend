using System.ComponentModel.DataAnnotations;
using Docentify.Domain.Entities.Courses;
using Docentify.Domain.Entities.Step;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.User;

[Table("users")]
[Index("Document", Name = "document_UNIQUE", IsUnique = true)]
[Index("Email", Name = "email_UNIQUE", IsUnique = true)]
public class UserEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Column("birthDate", TypeName = "date")]
    public DateTime BirthDate { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("telephone")]
    [StringLength(45)]
    public string? Telephone { get; set; }

    [Column("gender")]
    [StringLength(2)]
    public string? Gender { get; set; }

    [Column("document")]
    [StringLength(45)]
    public string Document { get; set; } = null!;

    [Column("creationDate", TypeName = "datetime")]
    public DateTime? CreationDate { get; set; }

    [Column("updateDate", TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<EnrollmentEntity> Enrollments { get; set; } = new List<EnrollmentEntity>();

    [InverseProperty("User")]
    public virtual ICollection<FavoriteEntity> Favorites { get; set; } = new List<FavoriteEntity>();
    
    // [InverseProperty("User")]
    // public virtual ICollection<Usernotification> Usernotifications { get; set; } = new List<Usernotification>();

    [InverseProperty("User")]
    public virtual UserPasswordHashEntity? UserPasswordHash { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<UserPreferencesValueEntity> UserPreferencesValues { get; set; } = new List<UserPreferencesValueEntity>();

    [InverseProperty("User")]
    public virtual UserScoreEntity? UserScore { get; set; }
    
    [InverseProperty("Users")]
    public virtual ICollection<InstitutionEntity> Institutions { get; set; } = new List<InstitutionEntity>();
    
    [InverseProperty("User")]
    public virtual ICollection<AttemptEntity> Attempts { get; set; } = new List<AttemptEntity>();
}
