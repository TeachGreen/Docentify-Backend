using System.ComponentModel.DataAnnotations;
using Docentify.Domain.Entities.User;

namespace Docentify.Domain.Entities.Step;

public class AttemptEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("score")]
    public int Score { get; set; }
    
    [Column("date")]
    public DateTime Date { get; set; }
    
    [Column("userId")]
    public int UserId { get; set; }
    
    [Column("activityId")]
    public int ActivityId { get; set; }
    
    [ForeignKey("UserId")]
    [InverseProperty("Attempts")]
    public virtual UserEntity User { get; set; }
    
    [ForeignKey("ActivityId")]
    [InverseProperty("Attempts")]
    public virtual ActivityEntity Activity { get; set; }
}