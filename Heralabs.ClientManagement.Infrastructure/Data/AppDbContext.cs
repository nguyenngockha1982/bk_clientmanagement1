using Heralabs.ClientManagement.Domain.Common;
using Heralabs.ClientManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Heralabs.ClientManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<WebApiVersion> WebApiVersions { get; set; }
        public DbSet<MobileAppVersion> MobileAppVersions { get; set; }
        public DbSet<ClientDeployment> ClientDeployments { get; set; }
        public DbSet<DeviceTracking> DeviceTrackings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình bảng Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("Client");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ClientCode).IsUnique();

                entity.Property(e => e.ClientCode).IsRequired().HasMaxLength(50).IsUnicode(false); // VARCHAR(50)
                entity.Property(e => e.ClientName).IsRequired().HasMaxLength(200); // NVARCHAR(200)
                entity.Property(e => e.ClientSecretKey).IsRequired().HasMaxLength(100).IsUnicode(false); // VARCHAR(100)
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.UpdatedBy).HasMaxLength(50).IsUnicode(false);
            });

            // 2. Cấu hình bảng WebApiVersion
            modelBuilder.Entity<WebApiVersion>(entity =>
            {
                entity.ToTable("WebApiVersion");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Version);

                entity.Property(e => e.Version).IsRequired().HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.MinimumMobileAppVersion).IsRequired().HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.MaximumMobileAppVersion).HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.UpdatedBy).HasMaxLength(50).IsUnicode(false);
            });

            // 3. Cấu hình bảng MobileAppVersion
            modelBuilder.Entity<MobileAppVersion>(entity =>
            {
                entity.ToTable("MobileAppVersion");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Version);

                // Lưu Enum AppPlatform dưới dạng chuỗi VARCHAR(50) thay vì số nguyên
                entity.Property(e => e.AppPlatform)
                      .HasConversion<string>()
                      .IsRequired()
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.Version).IsRequired().HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.DownloadPackageUrl).HasMaxLength(250);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.UpdatedBy).HasMaxLength(50).IsUnicode(false);
            });

            // 4. Cấu hình bảng ClientDeployment
            modelBuilder.Entity<ClientDeployment>(entity =>
            {
                entity.ToTable("ClientDeployment");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.EnvironmentName).HasMaxLength(50);
                entity.Property(e => e.ApiContainerName).HasMaxLength(200);
                entity.Property(e => e.WebApiVersionCode).HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.ApiUrl).HasMaxLength(500);

                entity.Property(e => e.WebContainerName).HasMaxLength(200);
                entity.Property(e => e.WebVersion).HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.WebUrl).HasMaxLength(500);

                entity.Property(e => e.SqlContainerName).HasMaxLength(200);
                entity.Property(e => e.SqlHeralabsDbName).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.SqlHeralabsDbUser).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.SqlHeralabsDbPasswordEncrypted).HasMaxLength(500).IsUnicode(false);

                entity.Property(e => e.PostgresContainerName).HasMaxLength(200);
                entity.Property(e => e.PostgresHeralabsDbName).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.PostgresHeralabsDbUser).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.PostgresHeralabsDbPasswordEncrypted).HasMaxLength(500).IsUnicode(false);

                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.UpdatedBy).HasMaxLength(50).IsUnicode(false);
            });

            // 5. Cấu hình bảng DeviceTracking
            modelBuilder.Entity<DeviceTracking>(entity =>
            {
                entity.ToTable("DeviceTracking");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ClientCode);

                entity.Property(e => e.ClientCode).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.Os).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.AppVersion).HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.DeviceBrand).HasMaxLength(250);
                entity.Property(e => e.DeviceModel).HasMaxLength(250);
                entity.Property(e => e.DeviceId).HasMaxLength(250);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseAuditableEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Created = DateTime.UtcNow;
                    if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                        entry.Entity.CreatedBy = "System";
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.Updated = DateTime.UtcNow;
                    if (string.IsNullOrEmpty(entry.Entity.UpdatedBy))
                        entry.Entity.UpdatedBy = "System";
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}