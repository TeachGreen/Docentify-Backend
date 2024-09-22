namespace Docentify.Domain.Entities;

[Table("UserPasswordHashes")]
public class UserPasswordHashEntity : BaseEntity
{
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
    public int UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public virtual UserEntity User { get; set; }
}