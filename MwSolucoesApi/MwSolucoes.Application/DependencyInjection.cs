using Microsoft.Extensions.DependencyInjection;
using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.Services;

namespace MwSolucoes.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            AddUseCases(services);
        }
        public static void AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<IServiceRequestService, ServiceRequestService>();
        }
    }
}
