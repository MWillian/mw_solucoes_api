using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Domain.Repositories
{
    public interface IServiceRequestRepository
    {
        Task Add(ServiceRequest serviceRequest);
        Task<bool> TryAdd(ServiceRequest serviceRequest);
        Task<ServiceRequest?> GetById(Guid id);
        Task<ServiceRequest?> GetByProtocol(string protocol);
    }
}
