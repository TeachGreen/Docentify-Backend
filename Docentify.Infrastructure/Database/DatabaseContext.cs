namespace Docentify.Infrastructure.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserPasswordHashEntity> PasswordHashes { get; set; }
    
    public DbSet<ActivityEntity> Activities { get; set; }

    public DbSet<CourseEntity> Courses { get; set; }

    public DbSet<CourseStyleEntity> Coursestyles { get; set; }
    
    public DbSet<EnrollmentEntity> Enrollments { get; set; }
    
    public DbSet<FileStepEntity> Filesteps { get; set; }

    public DbSet<InstitutionEntity> Institutions { get; set; }

    public DbSet<InstitutionPasswordHashEntity> Institutionpasswordhashes { get; set; }

    public DbSet<OptionEntity> Options { get; set; }

    public DbSet<QuestionEntity> Questions { get; set; }

    public DbSet<StepEntity> Steps { get; set; }

    public DbSet<StyleVariableEntity> Stylevariables { get; set; }

    public DbSet<StyleVariablesValueEntity> Stylevariablesvalues { get; set; }
    
    public DbSet<UserPasswordHashEntity> Userpasswordhashes { get; set; }

    public DbSet<UserPreferenceEntity> Userpreferences { get; set; }

    public DbSet<UserPreferencesValueEntity> Userpreferencesvalues { get; set; }

    public DbSet<UserProgressEntity> Userprogresses { get; set; }

    public DbSet<UserScoreEntity> Userscores { get; set; }

    public DbSet<VideoStepEntity> Videosteps { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityEntity>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.StepId }).HasName("PRIMARY");

            entity.ToTable("activities");

            entity.HasIndex(e => e.StepId, "stepId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.StepId).HasColumnName("stepId");

            entity.HasOne(d => d.Step).WithMany(p => p.Activities)
                .HasForeignKey(d => d.StepId)
                .HasConstraintName("activities_ibfk_1");
        });

        modelBuilder.Entity<CourseEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("courses");

            entity.HasIndex(e => e.InstitutionId, "institutionId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.InstitutionId).HasColumnName("institutionId");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");

            entity.HasOne(d => d.Institution).WithMany(p => p.Courses)
                .HasForeignKey(d => d.InstitutionId)
                .HasConstraintName("courses_ibfk_1");
        });

        modelBuilder.Entity<CourseStyleEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("coursestyles");

            entity.HasIndex(e => e.CourseId, "courseId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseStyles)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("coursestyles_ibfk_1");
        });

        modelBuilder.Entity<EnrollmentEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("enrollments");

            entity.HasIndex(e => e.CourseId, "courseId");

            entity.HasIndex(e => e.UserId, "userId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("enrollmentDate");
            entity.Property(e => e.IsRequired)
                .HasDefaultValueSql("'0'")
                .HasColumnName("isRequired");
            entity.Property(e => e.RequiredDate)
                .HasColumnType("datetime")
                .HasColumnName("requiredDate");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("enrollments_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("enrollments_ibfk_1");
        });

        modelBuilder.Entity<FileStepEntity>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.StepId }).HasName("PRIMARY");

            entity.ToTable("filesteps");

            entity.HasIndex(e => e.StepId, "stepId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.StepId).HasColumnName("stepId");
            entity.Property(e => e.Data)
                .HasColumnType("blob")
                .HasColumnName("data");

            entity.HasOne(d => d.Step).WithMany(p => p.Filesteps)
                .HasForeignKey(d => d.StepId)
                .HasConstraintName("filesteps_ibfk_1");
        });

        modelBuilder.Entity<InstitutionEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("institutions");

            entity.HasIndex(e => e.Email, "email_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Name, "name_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Telephone)
                .HasMaxLength(45)
                .HasColumnName("telephone");
        });

        modelBuilder.Entity<InstitutionPasswordHashEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("institutionpasswordhashes");

            entity.HasIndex(e => e.InstitutionId, "institutionId");

            entity.Property(e => e.Id)
                .HasMaxLength(45)
                .HasColumnName("id");
            entity.Property(e => e.HashedPassword)
                .HasMaxLength(200)
                .HasColumnName("hashedPassword");
            entity.Property(e => e.InstitutionId).HasColumnName("institutionId");
            entity.Property(e => e.Salt)
                .HasMaxLength(200)
                .HasColumnName("salt");

            entity.HasOne(d => d.Institution).WithMany(p => p.InstitutionPasswordHashes)
                .HasForeignKey(d => d.InstitutionId)
                .HasConstraintName("institutionpasswordhashes_ibfk_1");
        });

        modelBuilder.Entity<OptionEntity>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.QuestionId }).HasName("PRIMARY");

            entity.ToTable("options");

            entity.HasIndex(e => e.QuestionId, "questionId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.QuestionId).HasColumnName("questionId");
            entity.Property(e => e.IsCorrect)
                .HasDefaultValueSql("'0'")
                .HasColumnName("isCorrect");
            entity.Property(e => e.Text)
                .HasColumnType("text")
                .HasColumnName("text");
        });

        modelBuilder.Entity<QuestionEntity>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.ActivityId }).HasName("PRIMARY");

            entity.ToTable("questions");

            entity.HasIndex(e => e.ActivityId, "activityId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.ActivityId).HasColumnName("activityId");
            entity.Property(e => e.Statement)
                .HasColumnType("text")
                .HasColumnName("statement");
        });

        modelBuilder.Entity<StepEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("steps");

            entity.HasIndex(e => e.CourseId, "courseId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.Description)
                .HasMaxLength(45)
                .HasColumnName("description");
            entity.Property(e => e.Order).HasColumnName("order");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.Course).WithMany(p => p.Steps)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("steps_ibfk_1");
        });

        modelBuilder.Entity<StyleVariableEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("stylevariables");

            entity.HasIndex(e => e.Name, "name_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DefaultValue)
                .HasMaxLength(45)
                .HasColumnName("defaultValue");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StyleVariablesValueEntity>(entity =>
        {
            entity.HasKey(e => new { e.StyleId, e.VariableId }).HasName("PRIMARY");

            entity.ToTable("stylevariablesvalues");

            entity.HasIndex(e => e.VariableId, "variable_id");

            entity.Property(e => e.StyleId).HasColumnName("style_id");
            entity.Property(e => e.VariableId).HasColumnName("variable_id");
            entity.Property(e => e.Value)
                .HasMaxLength(45)
                .HasColumnName("value");

            entity.HasOne(d => d.Style).WithMany(p => p.Stylevariablesvalues)
                .HasForeignKey(d => d.StyleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stylevariablesvalues_ibfk_1");

            entity.HasOne(d => d.Variable).WithMany(p => p.Stylevariablesvalues)
                .HasForeignKey(d => d.VariableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stylevariablesvalues_ibfk_2");
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Document, "document_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Email, "email_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BirthDate)
                .HasColumnType("date")
                .HasColumnName("birthDate");
            entity.Property(e => e.Document)
                .HasMaxLength(45)
                .HasColumnName("document");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Telephone)
                .HasMaxLength(45)
                .HasColumnName("telephone");
            
            entity.HasMany(d => d.Institutions).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "Association",
                    r => r.HasOne<InstitutionEntity>().WithMany()
                        .HasForeignKey("InstitutionId")
                        .HasConstraintName("associations_ibfk_2"),
                    l => l.HasOne<UserEntity>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("associations_ibfk_1"),
                    j =>
                    {
                        j.HasKey("UserId", "InstitutionId").HasName("PRIMARY");
                        j.ToTable("associations");
                        j.HasIndex(new[] { "InstitutionId" }, "institutionId");
                        j.IndexerProperty<int>("UserId").HasColumnName("userId");
                        j.IndexerProperty<int>("InstitutionId").HasColumnName("institutionId");
                    });
        });

        modelBuilder.Entity<UserPasswordHashEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("userpasswordhashes");

            entity.HasIndex(e => e.UserId, "userId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.HashedPassword)
                .HasMaxLength(200)
                .HasColumnName("hashedPassword");
            entity.Property(e => e.Salt)
                .HasMaxLength(200)
                .HasColumnName("salt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithOne(p => p.UserPasswordHash)
                .HasConstraintName("userpasswordhashes_ibfk_1");
        });

        modelBuilder.Entity<UserPreferenceEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("userpreferences");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DefaultValue)
                .HasMaxLength(45)
                .HasColumnName("defaultValue");
            entity.Property(e => e.PreferenceName)
                .HasMaxLength(50)
                .HasColumnName("preferenceName");
        });

        modelBuilder.Entity<UserPreferencesValueEntity>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PreferenceId }).HasName("PRIMARY");

            entity.ToTable("userpreferencesvalues");

            entity.HasIndex(e => e.PreferenceId, "preferenceId");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.PreferenceId).HasColumnName("preferenceId");
            entity.Property(e => e.Value)
                .HasMaxLength(45)
                .HasColumnName("value");

            entity.HasOne(d => d.Preference).WithMany(p => p.Userpreferencesvalues)
                .HasForeignKey(d => d.PreferenceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userpreferencesvalues_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.UserPreferencesValues)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userpreferencesvalues_ibfk_1");
        });

        modelBuilder.Entity<UserProgressEntity>(entity =>
        {
            entity.HasKey(e => new { e.EnrollmentId, e.StepId }).HasName("PRIMARY");

            entity.ToTable("userprogress");

            entity.HasIndex(e => e.StepId, "stepId");

            entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");
            entity.Property(e => e.StepId).HasColumnName("stepId");
            entity.Property(e => e.ProgressDate)
                .HasColumnType("datetime")
                .HasColumnName("progressDate");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.UserProgresses)
                .HasForeignKey(d => d.EnrollmentId)
                .HasConstraintName("userprogress_ibfk_2");

            entity.HasOne(d => d.Step).WithMany(p => p.Userprogresses)
                .HasForeignKey(d => d.StepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userprogress_ibfk_1");
        });

        modelBuilder.Entity<UserScoreEntity>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("userscores");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Score)
                .HasDefaultValueSql("'0'")
                .HasColumnName("score");

            entity.HasOne(d => d.User).WithOne(p => p.UserScore)
                .HasForeignKey<UserScoreEntity>(d => d.UserId)
                .HasConstraintName("userscores_ibfk_1");
        });

        modelBuilder.Entity<VideoStepEntity>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.StepId }).HasName("PRIMARY");

            entity.ToTable("videosteps");

            entity.HasIndex(e => e.StepId, "stepId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.StepId).HasColumnName("stepId");
            entity.Property(e => e.Url)
                .HasMaxLength(250)
                .HasColumnName("url");

            entity.HasOne(d => d.Step).WithMany(p => p.Videosteps)
                .HasForeignKey(d => d.StepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("videosteps_ibfk_1");
        });
    }
}