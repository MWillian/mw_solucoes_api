using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.MaintenanceService;

namespace MwSolucoes.Application.Interfaces
{
    public interface IMaintenanceService
    {
        Task<ResponseCreateMaintenanceService> CreateMaintenanceService(RequestCreateMaintenanceService request);
        Task<ResponseGetMaintenanceService> GetMaintenanceServiceById(int id, bool isTechnician);
        Task<PagedResult<ResponseGetMaintenanceService>> GetMaintenanceServices(MaintenanceServiceFilters filters, bool isTechnician);
        Task<ResponseUpdateMaintenanceService> UpdateMaintenanceService(int id, RequestUpdateMaintenanceService request);
        Task DeactivateMaintenanceService (int id);
        Task DeleteMaintenanceService(int id);
        Task Reactivate(int id);
    }
}
