using System.ComponentModel.DataAnnotations;

namespace Docentify.Domain.Common.Entities;

public class PasswordHash
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("hashedPassword")]
    [StringLength(100)]
    public string HashedPassword { get; set; } = null!;

    [Column("salt")]
    [StringLength(100)]
    public string Salt { get; set; } = null!;
}