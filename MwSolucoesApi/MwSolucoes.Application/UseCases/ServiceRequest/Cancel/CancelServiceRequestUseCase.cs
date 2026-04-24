using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Enums;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;
using System;
using System.Threading.Tasks;

namespace MwSolucoes.Application.UseCases.ServiceRequest.Cancel
{
    public class CancelServiceRequestUseCase : ICancelServiceRequestUseCase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public CancelServiceRequestUseCase(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<ResponseUpdateServiceRequest> Execute(Guid serviceRequestId)
        {
            ValidateGuid(serviceRequestId);
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId)
                ?? throw new NotFoundException("Solicitacao de servico nao encontrada.");
            serviceRequest.Cancel();
            await _serviceRequestRepository.Update(serviceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
        }

        private static void ValidateGuid(Guid serviceRequestId)
        {
            if (serviceRequestId == Guid.Empty)
                throw new ErrorOnValidationException("O ID da solicitacao de servico e obrigatorio.");
        }
    }
}
