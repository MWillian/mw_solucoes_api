using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories.Filters;

namespace MwSolucoes.Domain.Repositories
{
    public interface IServiceRequestRepository
    {
        Task<bool> TryAdd(ServiceRequest serviceRequest);
        Task<ServiceRequest?> GetById(Guid id);
        Task<ServiceRequest?> GetByProtocol(string protocol);
        Task<PagedResult<ServiceRequest>> GetAll(ServiceRequestFilters filters, Guid userId, bool isQueue);
        Task DeleteById(Guid id);
        Task Update(ServiceRequest serviceRequest);
    }
}
