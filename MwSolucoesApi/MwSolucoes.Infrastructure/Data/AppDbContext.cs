using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<MaintenanceService> MaintenanceServices => Set<MaintenanceService>();
        public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
        public DbSet<ServiceRequestItem> ServiceRequestItems => Set<ServiceRequestItem>();
        public DbSet<ServiceRequestHistory> ServiceRequestHistories => Set<ServiceRequestHistory>();
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(70);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(40);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.AccessLevel).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.PhoneNumber)
                    .HasConversion(
                        valueObject => valueObject.Number,
                        value => new Domain.ValueObjects.PhoneNumber(value))
                    .HasMaxLength(11)
                    .IsRequired();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();

                entity.Property(e => e.CPF)
                    .HasConversion(
                        valueObject => valueObject.Number,
                        value => new Domain.ValueObjects.Cpf(value))
                    .HasMaxLength(11)
                    .IsRequired();

                entity.OwnsOne(e => e.Address, address =>
                {
                    address.Property(a => a.Logradouro).HasColumnName("Logradouro").HasMaxLength(120).IsRequired();
                    address.Property(a => a.Numero).HasColumnName("Numero").HasMaxLength(20).IsRequired();
                    address.Property(a => a.Bairro).HasColumnName("Bairro").HasMaxLength(80).IsRequired();
                    address.Property(a => a.Cidade).HasColumnName("Cidade").HasMaxLength(80).IsRequired();
                    address.Property(a => a.Estado).HasColumnName("Estado").HasMaxLength(2).IsRequired();
                    address.Property(a => a.Cep).HasColumnName("Cep").HasMaxLength(8).IsRequired();
                });

                entity.Navigation(e => e.Address).IsRequired();
            });

            modelBuilder.Entity<MaintenanceService>(entity =>
            {
                entity.ToTable("MaintenanceServices");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.Category).IsRequired().HasConversion<int>();
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.ToTable("ServiceRequests");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Protocol).IsRequired().HasMaxLength(20);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasConversion<int>();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.EquipmentType).IsRequired().HasConversion<int>();
                entity.Property(e => e.BrandModel).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ReportedProblem).IsRequired().HasColumnType("text");
                entity.Property(e => e.TechnicalDiagnosis).HasColumnType("text");
                entity.Property(e => e.LaborCost).HasPrecision(10, 2);
                entity.Property(e => e.PartsCost).HasPrecision(10, 2);
                entity.Property(e => e.RequiresDownPayment).IsRequired().HasDefaultValue(false);

                entity.HasIndex(e => e.Protocol).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Items)
                    .WithOne(e => e.ServiceRequest)
                    .HasForeignKey(e => e.ServiceRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ServiceRequestItem>(entity =>
            {
                entity.ToTable("ServiceRequestItems");
                entity.HasKey(e => new { e.ServiceRequestId, e.MaintenanceServiceId });

                entity.Property(e => e.UnitPrice).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.Quantity).IsRequired();

                entity.HasOne(e => e.MaintenanceService)
                    .WithMany()
                    .HasForeignKey(e => e.MaintenanceServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ServiceRequestHistory>(entity =>
            {
                entity.ToTable("ServiceRequestHistory");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.ServiceRequestId).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasConversion<int>();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.LastUpdatedAt);
                entity.HasOne(e => e.ServiceRequest)
                    .WithMany(e => e.Histories)
                    .HasForeignKey(e => e.ServiceRequestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
