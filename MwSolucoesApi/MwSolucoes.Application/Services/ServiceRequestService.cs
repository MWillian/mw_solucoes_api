using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Enums;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;


namespace MwSolucoes.Application.Services
{
    public class ServiceRequestService : IServiceRequestService

    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;
        private readonly IUserRepository _userRepository;

        public ServiceRequestService(IServiceRequestRepository serviceRequestRepository, IMaintenanceServiceRepository maintenanceServiceRepository, IUserRepository userRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _maintenanceServiceRepository = maintenanceServiceRepository;
            _userRepository = userRepository;
        }

        // Main methods
        public async Task<ResponseUpdateServiceRequest> AcceptServiceRequest(Guid serviceRequestId)
        {
            var ServiceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");
            await _serviceRequestRepository.Update(ServiceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(ServiceRequest);
        }

        public async Task<ResponseCreateServiceRequest> CreateServiceRequest(RequestCreateServiceRequest request, Guid userId)
        {
            ValidateRequest(request);
            User? user = await _userRepository.GetById(userId) ?? throw new NotFoundException("Usuário não encontrado.");
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

        public async Task<ResponseUpdateServiceRequest> CancelServiceRequest(Guid serviceRequestId)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId)
                ?? throw new NotFoundException("Solicitação de serviço não encontrada.");
            serviceRequest.Cancel();
            await _serviceRequestRepository.Update(serviceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
        }

        public async Task DeleteServiceRequest(Guid serviceRequestId, Guid userId, bool canViewAll)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");

            if (serviceRequest.Status != ServiceRequestStatus.Created)
                throw new ErrorOnValidationException("A solicitação de serviço deve ter status Criado para ser removida.");

            if (!canViewAll && serviceRequest.UserId != userId)
                throw new NotFoundException("Solicitação de serviço não encontrada.");

            await _serviceRequestRepository.DeleteById(serviceRequestId);
        }

        public async Task<ResponseUpdateServiceRequest> FinishServiceRequest(Guid serviceRequestId)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId)
                ?? throw new NotFoundException("Solicitação de serviço não encontrada.");

            serviceRequest.Finish();
            await _serviceRequestRepository.Update(serviceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
        }

        public async Task<Communication.Responses.PagedResult<ResponseGetServiceRequest>> GetServiceRequests(RequestGetServiceRequests filters, Guid userId, bool canViewAll)
        {
            filters ??= new RequestGetServiceRequests();

            filters.Page = filters.Page <= 0 ? 1 : filters.Page;
            filters.PageSize = filters.PageSize <= 0 ? 20 : Math.Min(filters.PageSize, 100);
            filters.SortBy = string.IsNullOrWhiteSpace(filters.SortBy) ? "createdAt" : filters.SortBy;
            filters.SortDirection = string.IsNullOrWhiteSpace(filters.SortDirection) ? "desc" : filters.SortDirection;

            var repositoryFilters = ServiceRequestMapper.MapToDomainFilters(filters);

            Guid? scopedUserId = canViewAll ? null : userId;
            var serviceRequests = await _serviceRequestRepository.GetAll(repositoryFilters, scopedUserId);
            return ServiceRequestMapper.ToResponseGetServiceRequests(serviceRequests);
        }

        public async Task<ResponseUpdateServiceRequest> RejectServiceRequest(Guid serviceRequestId)
        {
            var ServiceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");
            ServiceRequest.Reject();
            await _serviceRequestRepository.Update(ServiceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(ServiceRequest);
        }

        public async Task<ResponseUpdateServiceRequest> UpdateServiceRequest(Guid serviceRequestId, RequestUpdateServiceRequest request)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId)
            ?? throw new NotFoundException("Solicitação de serviço não encontrada.");

            serviceRequest.SetTechnicalData(request.TechnicalDiagnosis, request.LaborCost, request.PartsCost);
            await _serviceRequestRepository.Update(serviceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
        }

        public async Task<ResponseGetServiceRequest> GetServiceRequestById(Guid serviceRequestId, Guid userId, bool canViewAll)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");

            if (!canViewAll && serviceRequest.UserId != userId)
                throw new NotFoundException("Solicitação de serviço não encontrada.");

            return ServiceRequestMapper.ToResponseGetServiceRequest(serviceRequest);
        }

        //Helper Methods
        private static void ValidateRequest(RequestCreateServiceRequest request)
        {
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
