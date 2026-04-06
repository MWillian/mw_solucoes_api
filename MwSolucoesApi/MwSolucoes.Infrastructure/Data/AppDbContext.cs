using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
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
        }
    }
}
