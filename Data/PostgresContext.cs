using System;
using System.Collections.Generic;
using CDO.Models;
using Microsoft.EntityFrameworkCore;

namespace CDO.Data;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Discipline> Disciplines { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<LearningMaterial> LearningMaterials { get; set; }

    public virtual DbSet<Progress> Progresses { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<Theme> Themes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAnswer> UserAnswers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=adm;Password=admpassword");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_catalog", "adminpack");

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Answercd).HasName("answer_pkey");

            entity.ToTable("answer", "CDO");

            entity.Property(e => e.Answercd)
                .ValueGeneratedNever()
                .HasColumnName("answercd");
            entity.Property(e => e.Answernm)
                .HasMaxLength(50)
                .HasColumnName("answernm");
            entity.Property(e => e.Isright).HasColumnName("isright");
            entity.Property(e => e.Questioncd).HasColumnName("questioncd");

            entity.HasOne(d => d.QuestioncdNavigation).WithMany(p => p.Answers)
                .HasForeignKey(d => d.Questioncd)
                .HasConstraintName("answer_questioncd_fkey");
        });

        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.HasKey(e => e.Disciplinecd).HasName("discipline_pkey");

            entity.ToTable("discipline", "CDO");

            entity.HasIndex(e => e.Disciplinename, "discipline_disciplinename_key").IsUnique();

            entity.Property(e => e.Disciplinecd)
                .ValueGeneratedNever()
                .HasColumnName("disciplinecd");
            entity.Property(e => e.Disciplinename)
                .HasMaxLength(30)
                .HasColumnName("disciplinename");
            entity.Property(e => e.Usercd).HasColumnName("usercd");

            entity.HasOne(d => d.UsercdNavigation).WithMany(p => p.Disciplines)
                .HasForeignKey(d => d.Usercd)
                .HasConstraintName("discipline_usercd_fkey");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Groupcd).HasName("group_pkey");

            entity.ToTable("group", "CDO");

            entity.HasIndex(e => e.Number, "group_number_key").IsUnique();

            entity.Property(e => e.Groupcd)
                .ValueGeneratedNever()
                .HasColumnName("groupcd");
            entity.Property(e => e.FinishDate).HasColumnName("finish_date");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasMany(d => d.Disciplinecds).WithMany(p => p.Groupcds)
                .UsingEntity<Dictionary<string, object>>(
                    "GroupDiscipline",
                    r => r.HasOne<Discipline>().WithMany()
                        .HasForeignKey("Disciplinecd")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("group_disciplines_disciplinecd_fkey"),
                    l => l.HasOne<Group>().WithMany()
                        .HasForeignKey("Groupcd")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("group_disciplines_groupcd_fkey"),
                    j =>
                    {
                        j.HasKey("Groupcd", "Disciplinecd").HasName("group_disciplines_pkey");
                        j.ToTable("group_disciplines", "CDO");
                        j.IndexerProperty<int>("Groupcd").HasColumnName("groupcd");
                        j.IndexerProperty<int>("Disciplinecd").HasColumnName("disciplinecd");
                    });
        });

        modelBuilder.Entity<LearningMaterial>(entity =>
        {
            entity.HasKey(e => e.LearningMaterialcd).HasName("learning_material_pkey");

            entity.ToTable("learning_material", "CDO");

            entity.Property(e => e.LearningMaterialcd)
                .ValueGeneratedNever()
                .HasColumnName("learning_materialcd");
            entity.Property(e => e.Materialcontent)
                .HasMaxLength(50)
                .HasColumnName("materialcontent");
            entity.Property(e => e.Materialname)
                .HasMaxLength(30)
                .HasColumnName("materialname");
            entity.Property(e => e.Materialvolume).HasColumnName("materialvolume");
            entity.Property(e => e.Themecd).HasColumnName("themecd");

            entity.HasOne(d => d.ThemecdNavigation).WithMany(p => p.LearningMaterials)
                .HasForeignKey(d => d.Themecd)
                .HasConstraintName("learning_material_themecd_fkey");
        });

        modelBuilder.Entity<Progress>(entity =>
        {
            entity.HasKey(e => e.Progresscd).HasName("progress_pkey");

            entity.ToTable("progress", "CDO");

            entity.Property(e => e.Progresscd)
                .ValueGeneratedNever()
                .HasColumnName("progresscd");
            entity.Property(e => e.Progressdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("progressdate");
            entity.Property(e => e.Progressduration).HasColumnName("progressduration");
            entity.Property(e => e.Progressgrade).HasColumnName("progressgrade");
            entity.Property(e => e.Testcd).HasColumnName("testcd");
            entity.Property(e => e.Usercd).HasColumnName("usercd");

            entity.HasOne(d => d.TestcdNavigation).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.Testcd)
                .HasConstraintName("progress_testcd_fkey");

            entity.HasOne(d => d.UsercdNavigation).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.Usercd)
                .HasConstraintName("progress_usercd_fkey");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Questioncd).HasName("question_pkey");

            entity.ToTable("question", "CDO");

            entity.Property(e => e.Questioncd)
                .ValueGeneratedNever()
                .HasColumnName("questioncd");
            entity.Property(e => e.Questionnm)
                .HasMaxLength(50)
                .HasColumnName("questionnm");
            entity.Property(e => e.Testcd).HasColumnName("testcd");

            entity.HasOne(d => d.TestcdNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.Testcd)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("question_testcd_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Rolecd).HasName("role_pkey");

            entity.ToTable("role", "CDO");

            entity.Property(e => e.Rolecd)
                .ValueGeneratedNever()
                .HasColumnName("rolecd");
            entity.Property(e => e.Rolename)
                .HasMaxLength(20)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.Testcd).HasName("test_pkey");

            entity.ToTable("test", "CDO");

            entity.Property(e => e.Testcd)
                .ValueGeneratedNever()
                .HasColumnName("testcd");
            entity.Property(e => e.Disciplinecd).HasColumnName("disciplinecd");
            entity.Property(e => e.Testduration).HasColumnName("testduration");
            entity.Property(e => e.Testname)
                .HasMaxLength(50)
                .HasColumnName("testname");
            entity.Property(e => e.Testnumquest).HasColumnName("testnumquest");
            entity.Property(e => e.Themecd).HasColumnName("themecd");
            entity.Property(e => e.Usercd).HasColumnName("usercd");

            entity.HasOne(d => d.DisciplinecdNavigation).WithMany(p => p.Tests)
                .HasForeignKey(d => d.Disciplinecd)
                .HasConstraintName("test_disciplinecd_fkey");

            entity.HasOne(d => d.ThemecdNavigation).WithMany(p => p.Tests)
                .HasForeignKey(d => d.Themecd)
                .HasConstraintName("test_themecd_fkey");

            entity.HasOne(d => d.UsercdNavigation).WithMany(p => p.Tests)
                .HasForeignKey(d => d.Usercd)
                .HasConstraintName("test_usercd_fkey");
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.Themecd).HasName("theme_pkey");

            entity.ToTable("theme", "CDO");

            entity.Property(e => e.Themecd)
                .ValueGeneratedNever()
                .HasColumnName("themecd");
            entity.Property(e => e.Disciplinecd).HasColumnName("disciplinecd");
            entity.Property(e => e.Themename)
                .HasMaxLength(30)
                .HasColumnName("themename");
            entity.Property(e => e.Themevolume).HasColumnName("themevolume");

            entity.HasOne(d => d.DisciplinecdNavigation).WithMany(p => p.Themes)
                .HasForeignKey(d => d.Disciplinecd)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("theme_disciplinecd_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Usercd).HasName("user_pkey");

            entity.ToTable("user", "CDO");

            entity.Property(e => e.Usercd)
                .ValueGeneratedNever()
                .HasColumnName("usercd");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .HasColumnName("email");
            entity.Property(e => e.Fio)
                .HasMaxLength(50)
                .HasColumnName("fio");
            entity.Property(e => e.Groupcd).HasColumnName("groupcd");
            entity.Property(e => e.Login)
                .HasMaxLength(20)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .HasColumnName("password");
            entity.Property(e => e.Rolecd).HasColumnName("rolecd");
            entity.Property(e => e.Teacher).HasColumnName("teacher");

            entity.HasOne(d => d.GroupcdNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Groupcd)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_groupcd_fkey");

            entity.HasOne(d => d.RolecdNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Rolecd)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("user_rolecd_fkey");
        });

        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.HasKey(e => e.Useranswercd).HasName("user_answers_pkey");

            entity.ToTable("user_answers", "CDO");

            entity.Property(e => e.Useranswercd)
                .ValueGeneratedNever()
                .HasColumnName("useranswercd");
            entity.Property(e => e.Answercd).HasColumnName("answercd");
            entity.Property(e => e.Progresscd).HasColumnName("progresscd");
            entity.Property(e => e.Questioncd).HasColumnName("questioncd");

            entity.HasOne(d => d.AnswercdNavigation).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.Answercd)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("user_answers_answercd_fkey");

            entity.HasOne(d => d.ProgresscdNavigation).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.Progresscd)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("user_answers_progresscd_fkey");

            entity.HasOne(d => d.QuestioncdNavigation).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.Questioncd)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("user_answers_questioncd_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
