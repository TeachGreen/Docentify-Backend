using Docentify.Domain.Entities.Courses;
using Docentify.Domain.Entities.Step;
using Docentify.Domain.Entities.User;

namespace Docentify.Infrastructure.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    
    public DbSet<InstitutionEntity> Institutions { get; set; }

    public DbSet<InstitutionPasswordHashEntity> InstitutionPasswordHashes { get; set; }

    public DbSet<UserPasswordHashEntity> UserPasswordHashes { get; set; }

    public DbSet<UserPreferenceEntity> UserPreferences { get; set; }

    public DbSet<UserPreferencesValueEntity> UserPreferencesValues { get; set; }
    
    public DbSet<CourseEntity> Courses { get; set; }
    
    public DbSet<StepEntity> Steps { get; set; }
    
    public DbSet<ActivityEntity> Activities { get; set; }
    
    public DbSet<QuestionEntity> Questions { get; set; }
    
    public DbSet<AttemptEntity> Attempts { get; set; }
    
    public DbSet<UserProgressEntity> UserProgresses { get; set; }
    
    public DbSet<UserScoreEntity> UserScores { get; set; }
    
    public DbSet<PasswordChangeRequestEntity> PasswordChangeRequests { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
                .HasMaxLength(1)
                .HasColumnName("telephone");
            entity.Property(e => e.Document)
                .HasMaxLength(1)
                .HasColumnName("document");

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

            entity.HasOne(d => d.Institution).WithOne(p => p.InstitutionPasswordHash);
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
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("creationDate");
            entity.Property(e => e.Document)
                .HasMaxLength(45)
                .HasColumnName("document");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Telephone)
                .HasMaxLength(45)
                .HasColumnName("telephone");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updateDate");

            entity.HasMany(d => d.Institutions).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "Association",
                    r => r.HasOne<InstitutionEntity>().WithMany()
                        .HasForeignKey("InstitutionId"),
                    l => l.HasOne<UserEntity>().WithMany()
                        .HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "InstitutionId").HasName("PRIMARY");
                        j.ToTable("associations");
                        j.IndexerProperty<int>("UserId").HasColumnName("userId");
                        j.IndexerProperty<int>("InstitutionId").HasColumnName("institutionId");
                    });
        });
        
        modelBuilder.Entity<AttemptEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("activityattempts");

            entity.HasIndex(e => e.ActivityId, "activityId");

            entity.HasIndex(e => e.UserId, "userId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActivityId).HasColumnName("activityId");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Activity).WithMany(p => p.Attempts)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("activityattempts_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Attempts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("activityattempts_ibfk_1");
        });
        
        modelBuilder.Entity<UserScoreEntity>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");
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

            entity.HasOne(d => d.User).WithOne(p => p.UserPasswordHash);
        });

        modelBuilder.Entity<UserPreferenceEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("userpreferences");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DefaultValue)
                .HasMaxLength(45)
                .HasColumnName("defaultValue");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
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
            
            entity.HasOne(d => d.User).WithMany(p => p.UserPreferencesValues)
                .HasForeignKey(d => d.UserId);
        });
        
        modelBuilder.Entity<CourseEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdateDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Institution).WithMany(p => p.Courses);
        });
        
        modelBuilder.Entity<EnrollmentEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments);

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments);
        });
        
        modelBuilder.Entity<FavoriteEntity>(entity =>
        {
            entity.HasKey(e => new { e.CourseId, e.UserId }).HasName("PRIMARY");

            entity.Property(e => e.FavoriteDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Course).WithMany(p => p.Favorites);

            entity.HasOne(d => d.User).WithMany(p => p.Favorites);
        });

        modelBuilder.Entity<StepEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Course).WithMany(p => p.Steps);
        });
        
        modelBuilder.Entity<UserProgressEntity>(entity =>
        {
            entity.HasKey(e => new { e.EnrollmentId, e.StepId }).HasName("PRIMARY");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.UserProgresses).HasConstraintName("userprogress_ibfk_2");

            entity.HasOne(d => d.Step).WithMany(p => p.UserProgresses).HasConstraintName("userprogress_ibfk_1");
        });
    }
}