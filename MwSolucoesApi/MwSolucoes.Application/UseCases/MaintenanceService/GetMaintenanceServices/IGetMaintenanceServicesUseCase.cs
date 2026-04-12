using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.MaintenanceService;

namespace MwSolucoes.Application.UseCases.MaintenanceService.GetMaintenanceServices
{
    public interface IGetMaintenanceServicesUseCase
    {
        Task<PagedResult<ResponseGetMaintenanceService>> Execute(MaintenanceServiceFilters filters);
    }
}
