using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses.ServiceRequest;

namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public interface IUpdateServiceRequestUseCase
    {
        Task<ResponseUpdateServiceRequest> Execute(Guid serviceRequestId, RequestUpdateServiceRequest request);
    }
}
