using MwSolucoes.Communication.Responses.MaintenanceService;

namespace MwSolucoes.Application.UseCases.MaintenanceService.GetMaintenanceService
{
    public interface IGetMaintenanceServiceByIdUseCase
    {
        Task<ResponseGetMaintenanceService> Execute(int id);
    }
}
