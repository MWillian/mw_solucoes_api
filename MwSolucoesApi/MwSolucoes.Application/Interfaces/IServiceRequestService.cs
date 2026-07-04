using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.ServiceRequest;

namespace MwSolucoes.Application.Interfaces
{
    public interface IServiceRequestService
    {
        Task<ResponseUpdateServiceRequest> AcceptServiceRequest(Guid serviceRequestId);
        Task<ResponseCreateServiceRequest> CreateServiceRequest(RequestCreateServiceRequest request, Guid userId);
        Task<ResponseUpdateServiceRequest> CancelServiceRequest(Guid serviceRequestId);
        Task DeleteServiceRequest(Guid serviceRequestId, Guid userId, bool canViewAll);
        Task<ResponseUpdateServiceRequest> FinishServiceRequest(Guid serviceRequestId);
        Task<PagedResult<ResponseGetServiceRequest>> GetServiceRequests(RequestGetServiceRequests filters, Guid userId, bool canViewAll);
        Task<ResponseUpdateServiceRequest> RejectServiceRequest(Guid serviceRequestId);
        Task<ResponseUpdateServiceRequest> UpdateServiceRequest(Guid serviceRequestId, RequestUpdateServiceRequest request);
        Task<ResponseGetServiceRequest> GetServiceRequestById(Guid serviceRequestId, Guid userId, bool canViewAll);
    }
}
