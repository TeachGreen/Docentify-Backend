using System.ComponentModel.DataAnnotations;

namespace Docentify.Domain.Entities.User;

public class MultiFactorAuthenticationRequestEntity
{    
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("code")]
    public int Code { get; set; }
    
    [Column("creationDate", TypeName = "datetime")]
    public DateTime? CreationDate { get; set; }

    [Column("updateDate", TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }
    
    [ForeignKey("UserId")]
    [InverseProperty("MFARequests")]
    public virtual UserEntity User { get; set; } = null!;
}