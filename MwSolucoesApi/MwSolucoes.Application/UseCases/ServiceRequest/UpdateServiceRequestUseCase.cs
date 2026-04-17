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

            UpdateStatus(serviceRequest, request.Status);
            serviceRequest.SetTechnicalData(request.TechnicalDiagnosis, request.LaborCost, request.PartsCost);

            await _serviceRequestRepository.Update(serviceRequest);

            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
        }

        private static void Validate(Guid serviceRequestId, RequestUpdateServiceRequest request)
        {
            if (serviceRequestId == Guid.Empty)
                throw new ErrorOnValidationException("O ID da solicitação de serviço é obrigatório.");

            if (request is null)
                throw new ErrorOnValidationException("O objeto de requisição não pode ser nulo.");
        }

        private static void UpdateStatus(Domain.Entities.ServiceRequest serviceRequest, ServiceRequestStatus requestedStatus)
        {
            if (serviceRequest.Status == requestedStatus)
                return;

            switch (requestedStatus)
            {
                case ServiceRequestStatus.Created:
                    throw new ErrorOnValidationException("Não é permitido retornar a solicitação para o status Criado.");
                case ServiceRequestStatus.InProgress:
                    serviceRequest.StartProgress();
                    break;
                case ServiceRequestStatus.Finished:
                    serviceRequest.Finish();
                    break;
                case ServiceRequestStatus.Canceled:
                    serviceRequest.Cancel();
                    break;
                default:
                    throw new ErrorOnValidationException("Status de solicitação inválido.");
            }
        }
    }
}
