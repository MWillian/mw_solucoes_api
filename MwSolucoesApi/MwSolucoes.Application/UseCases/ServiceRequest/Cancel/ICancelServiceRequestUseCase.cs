using MwSolucoes.Communication.Responses.ServiceRequest;
using System;
using System.Threading.Tasks;

namespace MwSolucoes.Application.UseCases.ServiceRequest.Cancel
{
    public interface ICancelServiceRequestUseCase
    {
        Task<ResponseUpdateServiceRequest> Execute(Guid serviceRequestId);
    }
}
