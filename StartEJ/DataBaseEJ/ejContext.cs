using Microsoft.EntityFrameworkCore;

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

        public virtual DbSet<Educational> Educationals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=193.33.230.80;database=electrojournal;user id=Zhirov;password=64580082;SslMode=none", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.26-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<Educational>(entity =>
            {
                entity.HasKey(e => e.Ideducational)
                    .HasName("PRIMARY");

                entity.ToTable("educational");

                entity.HasIndex(e => e.Ideducational, "ideducational_institutions_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Ideducational).HasColumnName("ideducational");

                entity.Property(e => e.Name)
                    .HasColumnType("text")
                    .HasColumnName("name");

                entity.Property(e => e.NameDb)
                    .HasColumnType("text")
                    .HasColumnName("name_db");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
