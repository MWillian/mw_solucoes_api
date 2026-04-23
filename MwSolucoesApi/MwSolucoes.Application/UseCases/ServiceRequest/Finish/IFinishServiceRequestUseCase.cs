using MwSolucoes.Communication.Responses.ServiceRequest;
using System;
using System.Threading.Tasks;

namespace MwSolucoes.Application.UseCases.ServiceRequest.Finish
{
    public interface IFinishServiceRequestUseCase
    {
        Task<ResponseUpdateServiceRequest> Execute(Guid serviceRequestId);
    }
}
