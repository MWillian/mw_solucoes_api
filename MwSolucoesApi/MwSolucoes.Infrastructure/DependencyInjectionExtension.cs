using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Infrastructure.Data;
using MwSolucoes.Infrastructure.Repositories;

namespace MwSolucoes.Infrastructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string connectionString)
        {
            AddDbConnection(services, connectionString);
            AddRepositories(services);
        }

        private static void AddDbConnection(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
        }
        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();

        }
    }
}
