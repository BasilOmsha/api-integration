using api_integration.Domain.src.Entities.Fingrid;
using Microsoft.EntityFrameworkCore;

namespace api_integration.Infrastructure.src.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<MetaData> MetaDataCache => Set<MetaData>();
        public DbSet<CachedDataPoint> DataPointCache => Set<CachedDataPoint>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MetaData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.DatasetId).IsUnique();
                entity.Property(e => e.KeyWordsEn).HasColumnType("text[]");
                entity.Property(e => e.ContentGroupsEn).HasColumnType("text[]");
                entity.Property(e => e.AvailableFormats).HasColumnType("text[]");
                entity.OwnsOne(e => e.License);
            });

            modelBuilder.Entity<CachedDataPoint>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.DatasetId, e.StartTime }).IsUnique();
                entity.HasOne<MetaData>()
                .WithMany()
                .HasForeignKey(e => e.DatasetId)
                .HasPrincipalKey(e => e.DatasetId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}