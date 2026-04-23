using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Enums;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public class UpdateServiceRequestUseCase : IUpdateServiceRequestUseCase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public UpdateServiceRequestUseCase(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<ResponseUpdateServiceRequest> Execute(Guid serviceRequestId, RequestUpdateServiceRequest request)
        {
            Validate(serviceRequestId, request);

            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId)
                ?? throw new NotFoundException("Solicitação de serviço não encontrada.");

            serviceRequest.SetTechnicalData(request.TechnicalDiagnosis, request.LaborCost, request.PartsCost);
            await _serviceRequestRepository.Update(serviceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
        }

        private void Validate(Guid serviceRequestId, RequestUpdateServiceRequest request)
        {
            if (serviceRequestId == Guid.Empty)
                throw new ErrorOnValidationException("O ID da solicitação de serviço é obrigatório.");

            if (request is null)
                throw new ErrorOnValidationException("O objeto de requisição não pode ser nulo.");
        }
    }
}
