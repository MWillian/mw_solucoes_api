using Microsoft.Extensions.DependencyInjection;
using MwSolucoes.Application.UseCases.Auth;
using MwSolucoes.Application.UseCases.Auth.UpdatePassword;
using MwSolucoes.Application.UseCases.MaintenanceService.Create;
using MwSolucoes.Application.UseCases.MaintenanceService.Deactivate;
using MwSolucoes.Application.UseCases.MaintenanceService.Delete;
using MwSolucoes.Application.UseCases.MaintenanceService.GetMaintenanceService;
using MwSolucoes.Application.UseCases.MaintenanceService.GetMaintenanceServices;
using MwSolucoes.Application.UseCases.MaintenanceService.Update;
using MwSolucoes.Application.UseCases.ServiceRequest;
using MwSolucoes.Application.UseCases.User.DeleteUser;
using MwSolucoes.Application.UseCases.User.GetUser;
using MwSolucoes.Application.UseCases.User.GetUsers;
using MwSolucoes.Application.UseCases.User.Register;
using MwSolucoes.Application.UseCases.User.RegisterUser;
using MwSolucoes.Application.UseCases.User.UpdateUser;

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
            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            services.AddScoped<IGetUserByIdUseCase, GetUserByIdUseCase>();
            services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
            services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();
            services.AddScoped<ILoginUseCase, LoginUseCase>();
            services.AddScoped<IUpdatePasswordUseCase, UpdatePasswordUseCase>();
            services.AddScoped<ICreateMaintenanceServiceUseCase, CreateMaintenanceServiceUseCase>();
            services.AddScoped<IDeleteMaintenanceServiceUseCase, DeleteMaintenanceServiceUseCase>();
            services.AddScoped<IUpdateMaintenanceServiceUseCase, UpdateMaintenanceServiceUseCase>();
            services.AddScoped<IDeactivateMaintenanceServiceUseCase, DeactivateMaintenanceServiceUseCase>();
            services.AddScoped<IGetMaintenanceServicesUseCase, GetMaintenanceServicesUseCase>();
            services.AddScoped<IGetMaintenanceServiceByIdUseCase, GetMaintenanceServiceByIdUseCase>();
            services.AddScoped<ICreateServiceRequestUseCase, CreateServiceRequestUseCase>();
            services.AddScoped<IGetServiceRequestsUseCase, GetServiceRequestsUseCase>();
            services.AddScoped<IGetServiceRequestByIdUseCase, GetServiceRequestByIdUseCase>();
            services.AddScoped<IDeleteServiceRequestUseCase, DeleteServiceRequestUseCase>();
            services.AddScoped<IUpdateServiceRequestUseCase, UpdateServiceRequestUseCase>();
        }
    }
}
