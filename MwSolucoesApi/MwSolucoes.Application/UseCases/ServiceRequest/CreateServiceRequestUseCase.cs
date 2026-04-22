using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public class CreateServiceRequestUseCase : ICreateServiceRequestUseCase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;

        public CreateServiceRequestUseCase(IServiceRequestRepository serviceRequestRepository, IMaintenanceServiceRepository maintenanceServiceRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _maintenanceServiceRepository = maintenanceServiceRepository;
        }

        public async Task<ResponseCreateServiceRequest> Execute(RequestCreateServiceRequest request, Guid userId)
        {
            ValidateRequest(request, userId);
            var items = await BuildItems(request.ServiceIds);

            const int maxAttempts = 5;
            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                var serviceRequest = ServiceRequestMapper.ToServiceRequest(
                    request,
                    userId,
                    request.TechnicalDiagnosis,
                    request.LaborCost,
                    request.PartsCost,
                    items);

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

            if (request.ServiceIds is null || request.ServiceIds.Count == 0)
                throw new ErrorOnValidationException("A solicitação deve conter ao menos um serviço.");
        }

        private async Task<List<ServiceRequestItem>> BuildItems(List<int> serviceIds)
        {
            var distinctServiceIds = serviceIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (distinctServiceIds.Count == 0)
                throw new ErrorOnValidationException("Os ids dos serviços selecionados são inválidos.");

            var services = await _maintenanceServiceRepository.GetByIds(distinctServiceIds);

            if (services.Count != distinctServiceIds.Count)
                throw new NotFoundException("Um ou mais serviços selecionados não foram encontrados.");

            if (services.Any(service => !service.IsActive))
                throw new ErrorOnValidationException("Não é permitido solicitar serviços inativos.");

            return services
                .Select(service => new ServiceRequestItem(service.Id, service.Price))
                .ToList();
        }
    }
}
