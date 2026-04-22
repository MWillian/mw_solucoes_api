using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories.Filters;

namespace MwSolucoes.Domain.Repositories
{
    public interface IMaintenanceServiceRepository
    {
        Task Add(MaintenanceService service);
        Task Update(MaintenanceService service);
        Task Delete(MaintenanceService service);
        Task<PagedResult<MaintenanceService>> GetAll(MaintenanceServiceFilters filters);
        Task<MaintenanceService?> GetById(int id);
        Task<List<MaintenanceService>> GetByIds(List<int> ids);
        Task<MaintenanceService?> GetByName(string name);
    }
}
