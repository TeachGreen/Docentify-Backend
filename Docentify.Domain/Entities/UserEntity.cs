using System.ComponentModel.DataAnnotations;

namespace Docentify.Domain.Entities;

[Table("Users")]
public class UserEntity : BaseEntity
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
    
    [MaxLength(14)]
    public string? Telephone { get; set; }
    public string? Gender { get; set; }
    public string Document { get; set; } = null!;
    public DateTime? CreationDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    
    public virtual ICollection<InstitutionEntity> Institutions { get; set; } = new List<InstitutionEntity>();
    
    public virtual ICollection<EnrollmentEntity> Enrollments { get; set; } = new List<EnrollmentEntity>();

    public virtual ICollection<UserCardEntity> UserCards { get; set; } = new List<UserCardEntity>();

    public virtual ICollection<UserNotificationEntity> UserNotifications { get; set; } = new List<UserNotificationEntity>();
    
    public virtual ICollection<UserPreferencesValueEntity> UserPreferencesValues { get; set; } = new List<UserPreferencesValueEntity>();

    public virtual UserPasswordHashEntity UserPasswordHash { get; set; }
    
    public virtual UserScoreEntity? UserScore { get; set; }
}