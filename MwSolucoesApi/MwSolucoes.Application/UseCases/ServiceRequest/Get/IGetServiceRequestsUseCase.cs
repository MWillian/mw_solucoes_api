using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.ServiceRequest;

namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public interface IGetServiceRequestsUseCase
    {
        Task<PagedResult<ResponseGetServiceRequest>> Execute(RequestGetServiceRequests filters, Guid userId, bool canViewAll);
    }
}
