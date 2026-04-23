using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MwSolucoes.Communication.Responses.ServiceRequest;

namespace MwSolucoes.Application.UseCases.ServiceRequest.Accept
{
    public interface IAcceptServiceRequestUseCase
    {
        Task<ResponseUpdateServiceRequest> Execute(Guid serviceRequestId);
    }
}