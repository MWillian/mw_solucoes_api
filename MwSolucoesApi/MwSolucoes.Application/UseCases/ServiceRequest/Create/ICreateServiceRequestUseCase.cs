using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses.ServiceRequest;

namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public interface ICreateServiceRequestUseCase
    {
        Task<ResponseCreateServiceRequest> Execute(RequestCreateServiceRequest request, Guid userId);
    }
}
