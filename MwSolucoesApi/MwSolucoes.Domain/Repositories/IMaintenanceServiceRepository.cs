using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Domain.Repositories
{
    public interface IMaintenanceServiceRepository
    {
        Task Add(MaintenanceService service);
        Task Update(MaintenanceService service);
        Task Delete(MaintenanceService service);
        Task<PagedResult<MaintenanceService>> GetAll(int id);
        Task<MaintenanceService?> GetById(int id);
        Task<MaintenanceService?> GetByName(string name);
    }
}
