using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MwSolucoes.Domain.CepValidation;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Security.Cryptography;
using MwSolucoes.Domain.Security.Tokens;
using MwSolucoes.Infrastructure.CepValidation;
using MwSolucoes.Infrastructure.Data;
using MwSolucoes.Infrastructure.Repositories;
using MwSolucoes.Infrastructure.Security.Cryptography;
using MwSolucoes.Infrastructure.Security.Tokens;

namespace MwSolucoes.Infrastructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string connectionString)
        {
            AddDbConnection(services, connectionString);
            AddCepValidation(services, configuration);
            AddRepositories(services);
            AddPasswordEncrypter(services);
            AddToken(services, configuration);
        }

        private static void AddDbConnection(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
        }
        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMaintenanceServiceRepository, MaintenanceServiceRepository>();
            services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

        }
        private static void AddCepValidation(IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration["ViaCep:BaseUrl"] ?? "https://viacep.com.br/ws/";

            services.AddHttpClient<ICepValidator, CepValidator>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(5);
            });
        }
        public static void AddPasswordEncrypter(IServiceCollection services)
        {
            services.AddScoped<IPasswordEncrypter, Bcrypter>();
        }
        private static void AddToken(IServiceCollection services, IConfiguration configuration)
        {
            var expireTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpireMinutes");
            var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");
            var issuer = configuration.GetValue<string>("Settings:Jwt:Issuer") ?? string.Empty;
            var audience = configuration.GetValue<string>("Settings:Jwt:Audience") ?? string.Empty;
            services.AddScoped<ITokenGenerator>(config => new TokenGenerator(expireTimeMinutes, signingKey!, issuer, audience));
        }
    }
}
