using Microsoft.Extensions.DependencyInjection;
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
        }
    }
}
