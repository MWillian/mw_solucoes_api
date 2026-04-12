using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses.MaintenanceService;

namespace MwSolucoes.Application.UseCases.MaintenanceService.Create
{
    public interface ICreateMaintenanceServiceUseCase
    {
        Task<ResponseCreateMaintenanceService> Execute(RequestCreateMaintenanceService request);
    }
}
