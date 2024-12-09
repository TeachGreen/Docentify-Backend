using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.User;

[Table("userscores")]
[Index("UserId", Name = "userId")]
public class UserScoreEntity
{
    [Column("userId")]
    public int UserId { get; set; }
    
    [Column("score")]
    public int Score { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserScore")]
    public virtual UserEntity? User { get; set; } = null!;
}
