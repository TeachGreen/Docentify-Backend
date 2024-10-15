using System.ComponentModel.DataAnnotations;
using Docentify.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Courses;

[PrimaryKey("CourseId", "UserId")]
[Table("favoritedcourses")]
[Index("UserId", Name = "userId")]
public class FavoriteEntity
{
    [Key]
    [Column("courseId")]
    public int CourseId { get; set; }

    [Key]
    [Column("userId")]
    public int UserId { get; set; }

    [Column("favoriteDate", TypeName = "datetime")]
    public DateTime? FavoriteDate { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Favorites")]
    public virtual CourseEntity Course { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Favorites")]
    public virtual UserEntity User { get; set; } = null!;
}
