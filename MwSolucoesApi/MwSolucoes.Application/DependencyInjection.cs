using Microsoft.Extensions.DependencyInjection;
using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.Services;
using MwSolucoes.Application.UseCases.ServiceRequest;
using MwSolucoes.Application.UseCases.ServiceRequest.Accept;
using MwSolucoes.Application.UseCases.ServiceRequest.Cancel;
using MwSolucoes.Application.UseCases.ServiceRequest.Finish;
using MwSolucoes.Application.UseCases.ServiceRequest.Reject;

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

            services.AddScoped<ICreateServiceRequestUseCase, CreateServiceRequestUseCase>();
            services.AddScoped<IGetServiceRequestsUseCase, GetServiceRequestsUseCase>();
            services.AddScoped<IGetServiceRequestByIdUseCase, GetServiceRequestByIdUseCase>();
            services.AddScoped<IDeleteServiceRequestUseCase, DeleteServiceRequestUseCase>();
            services.AddScoped<IUpdateServiceRequestUseCase, UpdateServiceRequestUseCase>();
            services.AddScoped<IAcceptServiceRequestUseCase, AcceptServiceRequestUseCase>();
            services.AddScoped<IRejectServiceRequestUseCase, RejectServiceRequestUseCase>();
            services.AddScoped<IFinishServiceRequestUseCase, FinishServiceRequestUseCase>();
            services.AddScoped<ICancelServiceRequestUseCase, CancelServiceRequestUseCase>();
        }
    }
}
