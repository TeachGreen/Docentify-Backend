using System.ComponentModel.DataAnnotations;
using Docentify.Domain.Entities.Courses;
using Docentify.Domain.Entities.Step;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.User;

[Table("PasswordChangeRequests")]
public class PasswordChangeRequestEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;
    
    [Column("code")]
    [StringLength(100)]
    public string Code { get; set; } = null!;

    [Column("expiresAt", TypeName = "datetime")]
    public DateTime? ExpiresAt { get; set; }
}
