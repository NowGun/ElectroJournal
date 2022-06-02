using ElectroJournal.Classes.DataBaseEF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Xml;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class zhirovContext : DbContext
    {
        public zhirovContext()
        {
        }
        XmlDocument xmlDocument = new XmlDocument();
        public zhirovContext(DbContextOptions<zhirovContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Teacher> Teachers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
     
                xmlDocument.Load("setting.xml");

                string server = xmlDocument.GetElementsByTagName("server")[0].InnerText;
                string username = xmlDocument.GetElementsByTagName("username")[0].InnerText;
                string password = xmlDocument.GetElementsByTagName("password")[0].InnerText;
                string database = xmlDocument.GetElementsByTagName("database")[0].InnerText;

                // Connection String.
                String connString = $"Server={server};Database={database};User Id={username};password={password};SslMode=none";

                optionsBuilder.UseMySql(connString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.26-mysql"));

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.Idteachers)
                    .HasName("PRIMARY");

                entity.ToTable("teachers");

                entity.HasIndex(e => e.Idteachers, "idteachers_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.TeachersLogin, "teachers_login_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idteachers).HasColumnName("idteachers");

                entity.Property(e => e.TeachersAccesAdminPanel)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("teachers_acces_admin_panel");

                entity.Property(e => e.TeachersImage)
                    .HasColumnType("text")
                    .HasColumnName("teachers_image");

                entity.Property(e => e.TeachersLogin)
                    .HasMaxLength(45)
                    .HasColumnName("teachers_login");

                entity.Property(e => e.TeachersMail)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("teachers_mail");

                entity.Property(e => e.TeachersName)
                    .HasMaxLength(45)
                    .HasColumnName("teachers_name");

                entity.Property(e => e.TeachersPassword)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("teachers_password");

                entity.Property(e => e.TeachersPatronymic)
                    .HasMaxLength(45)
                    .HasColumnName("teachers_patronymic");

                entity.Property(e => e.TeachersPhone)
                    .HasMaxLength(12)
                    .HasColumnName("teachers_phone");

                entity.Property(e => e.TeachersStatus).HasColumnName("teachers_status");

                entity.Property(e => e.TeachersSurname)
                    .HasMaxLength(45)
                    .HasColumnName("teachers_surname");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
