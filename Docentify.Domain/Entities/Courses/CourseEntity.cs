using System.ComponentModel.DataAnnotations;
using Docentify.Domain.Entities.Step;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Domain.Entities.Courses;

[Table("courses")]
[Index("InstitutionId", Name = "institutionId")]
public class CourseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(45)]
    public string Name { get; set; } = null!;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }
    
    [Column("isRequired")]
    public bool? IsRequired { get; set; } = false;
    
    [Column("duration")]
    public int Duration { get; set; }
    
    [Column("requiredTimeLimit")]
    public int RequiredTimeLimit { get; set; } = 30;

    [Column("institutionId")]
    public int InstitutionId { get; set; }
    
    [Column("image")]
    public string Image { get; set; } = null!;

    [Column("creationDate", TypeName = "datetime")]
    public DateTime? CreationDate { get; set; }

    [Column("updateDate", TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    // [InverseProperty("Course")]
    // public virtual ICollection<Coursestyle> Coursestyles { get; set; } = new List<Coursestyle>();

    [InverseProperty("Course")]
    public virtual ICollection<EnrollmentEntity> Enrollments { get; set; } = new List<EnrollmentEntity>();

    [InverseProperty("Course")]
    public virtual ICollection<FavoriteEntity> Favorites { get; set; } = new List<FavoriteEntity>();

    [ForeignKey("InstitutionId")]
    [InverseProperty("Courses")]
    public virtual InstitutionEntity Institution { get; set; } = null!;

    [InverseProperty("Course")]
    public virtual ICollection<StepEntity> Steps { get; set; } = new List<StepEntity>();
}
