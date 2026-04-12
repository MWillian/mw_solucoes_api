using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses.MaintenanceService;

namespace MwSolucoes.Application.UseCases.MaintenanceService.Update
{
    public interface IUpdateMaintenanceServiceUseCase
    {
        Task<ResponseUpdateMaintenanceService> Execute(int id, RequestUpdateMaintenanceService request);
    }
}
