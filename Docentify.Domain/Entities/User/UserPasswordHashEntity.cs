using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.User;

[Table("userpasswordhashes")]
[Index("UserId", Name = "userId")]
public class UserPasswordHashEntity : PasswordHash
{
    [Column("userId")]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserPasswordHash")]
    public virtual UserEntity User { get; set; } = null!;
}
