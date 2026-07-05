using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Domain.Repositories
{
    public interface IServiceRequestHistoryRepository
    {
        Task UpdateHistory(ServiceRequest serviceRequest);
    }
}
