using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Application.Interfaces
{
    public interface IServiceRequestService
    {
        Task<ResponseUpdateServiceRequest> AcceptServiceRequest(Guid serviceRequestId, Guid technicianId);
        Task<ResponseCreateServiceRequest> CreateServiceRequest(RequestCreateServiceRequest request, Guid userId);
        Task<ResponseUpdateServiceRequest> CancelServiceRequest(Guid serviceRequestId, Guid userId);
        Task<ResponseUpdateServiceRequest> FinishServiceRequest(Guid serviceRequestId, Guid technicianId);
        Task<Communication.Responses.PagedResult<ResponseGetServiceRequest>> GetServiceRequests(RequestGetServiceRequests filters, Guid userId, bool isQueue);
        Task<ResponseUpdateServiceRequest> RejectServiceRequest(Guid serviceRequestId, Guid technicianId);
        Task<ResponseUpdateServiceRequest> UpdateServiceRequest(Guid serviceRequestId, RequestUpdateServiceRequest request, Guid technicianId);
        Task<ResponseGetServiceRequest> GetServiceRequestById(Guid serviceRequestId, Guid userId, bool isTechnician);
        Task<List<ResponseServiceRequestHistory>> GetTimeServiceRequestTimeline(Guid serviceRequestId, Guid userId);
        Task<byte[]> GenerateServiceRequestPdfAsync(Guid serviceRequestId, Guid userId, bool isTechnician);
    }
}
