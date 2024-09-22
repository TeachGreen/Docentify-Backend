namespace Docentify.Domain.Entities;

public class StepEntity : BaseEntity
{
    public int Order { get; set; }

    public string Description { get; set; } = null!;

    public int Type { get; set; }

    public int CourseId { get; set; }

    public virtual ICollection<ActivityEntity> Activities { get; set; } = new List<ActivityEntity>();

    public virtual CourseEntity Course { get; set; } = null!;

    public virtual ICollection<FileStepEntity> Filesteps { get; set; } = new List<FileStepEntity>();

    public virtual ICollection<UserProgressEntity> Userprogresses { get; set; } = new List<UserProgressEntity>();

    public virtual ICollection<VideoStepEntity> Videosteps { get; set; } = new List<VideoStepEntity>();
}
