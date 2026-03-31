using Microsoft.Extensions.DependencyInjection;
using MwSolucoes.Application.UseCases.User.RegisterUser;
using MwSolucoes.Application.UseCases.User.Register;

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
        }
    }
}
