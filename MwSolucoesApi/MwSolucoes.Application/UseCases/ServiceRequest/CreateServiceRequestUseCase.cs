using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public class CreateServiceRequestUseCase : ICreateServiceRequestUseCase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public CreateServiceRequestUseCase(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<ResponseCreateServiceRequest> Execute(RequestCreateServiceRequest request, Guid userId)
        {
            ValidateRequest(request, userId);

            const int maxAttempts = 5;
            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                var serviceRequest = ServiceRequestMapper.ToServiceRequest(
                    request,
                    userId,
                    request.TechnicalDiagnosis,
                    request.LaborCost,
                    request.PartsCost);

                var created = await _serviceRequestRepository.TryAdd(serviceRequest);
                if (created)
                    return ServiceRequestMapper.ToResponseCreateServiceRequest(serviceRequest);
            }

            throw new RequestConflictException("Não foi possível gerar um protocolo único para a solicitação. Tente novamente.");
        }

        private static void ValidateRequest(RequestCreateServiceRequest request, Guid userId)
        {
            if (request is null)
                throw new ErrorOnValidationException("O objeto de requisição não pode ser nulo.");

            if (userId == Guid.Empty)
                throw new ErrorOnValidationException("Usuário inválido para abertura da solicitação.");
        }
    }
}
