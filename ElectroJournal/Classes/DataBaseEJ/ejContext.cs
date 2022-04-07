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

        public virtual DbSet<Bugreporter> Bugreporters { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Version> Versions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=193.33.230.80;database=electrojournal;user id=Zhirov;password=64580082", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.28-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<Bugreporter>(entity =>
            {
                entity.HasKey(e => e.Idbugreporter)
                    .HasName("PRIMARY");

                entity.ToTable("bugreporter");

                entity.HasIndex(e => e.StatusIdstatus, "fk_bugreporter_status_idx");

                entity.HasIndex(e => e.Idbugreporter, "idbugreporter_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idbugreporter).HasColumnName("idbugreporter");

                entity.Property(e => e.BugreporterImage)
                    .HasColumnType("text")
                    .HasColumnName("bugreporter_image");

                entity.Property(e => e.BugreporterMessage)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("bugreporter_message");

                entity.Property(e => e.BugreporterTitle)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("bugreporter_title");

                entity.Property(e => e.StatusIdstatus).HasColumnName("status_idstatus");

                entity.HasOne(d => d.StatusIdstatusNavigation)
                    .WithMany(p => p.Bugreporters)
                    .HasForeignKey(d => d.StatusIdstatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_bugreporter_status");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(e => e.Idstatus)
                    .HasName("PRIMARY");

                entity.ToTable("status");

                entity.HasIndex(e => e.Idstatus, "idstatus_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idstatus).HasColumnName("idstatus");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("status_name");
            });

            modelBuilder.Entity<Version>(entity =>
            {
                entity.HasKey(e => e.Idversion)
                    .HasName("PRIMARY");

                entity.ToTable("version");

                entity.HasIndex(e => e.Idversion, "idversion_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idversion).HasColumnName("idversion");

                entity.Property(e => e.VersionName)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("Version_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
