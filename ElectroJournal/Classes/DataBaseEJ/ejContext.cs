using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ElectroJournal.Classes.DataBaseEJ
{
    public partial class ejContext : DbContext
    {
        public ejContext()
        {
        }

        public ejContext(DbContextOptions<ejContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Version> Versions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=80.240.250.128;database=electrojournal;user id=Zhirov;password=64580082", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.26-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Version>(entity =>
            {
                entity.HasKey(e => e.IdVersion)
                    .HasName("PRIMARY");

                entity.ToTable("version");

                entity.HasIndex(e => e.IdVersion, "idVersion_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.IdVersion).HasColumnName("idVersion");

                entity.Property(e => e.VersionName)
                    .HasMaxLength(45)
                    .HasColumnName("Version_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
