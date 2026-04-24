using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Enums;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.ServiceRequest.Accept
{
    public class AcceptServiceRequestUseCase : IAcceptServiceRequestUseCase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        public AcceptServiceRequestUseCase(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }
        public async Task<ResponseUpdateServiceRequest> Execute(Guid serviceRequestId)
        {
            ValidateGuid(serviceRequestId);
            var ServiceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");
            await _serviceRequestRepository.Update(ServiceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(ServiceRequest);
        }
        private void ValidateGuid(Guid serviceRequestId)
        {
            if (serviceRequestId == Guid.Empty)
                throw new ErrorOnValidationException("O ID da solicitação de serviço é obrigatório.");
        }
    }
}