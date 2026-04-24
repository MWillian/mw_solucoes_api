using MwSolucoes.Communication.Responses.ServiceRequest;

namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public interface IGetServiceRequestByIdUseCase
    {
        Task<ResponseGetServiceRequest> Execute(Guid serviceRequestId, Guid userId, bool canViewAll);
    }
}
