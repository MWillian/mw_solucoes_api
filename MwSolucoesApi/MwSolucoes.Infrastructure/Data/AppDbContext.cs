using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        internal DbSet<User> Users { get; set; }
    }
}
