using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Enums;
using MwSolucoes.Domain.PdfGenerator;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;


namespace MwSolucoes.Application.Services
{
    public class ServiceRequestService : IServiceRequestService

    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderServiceReportGenerator _pdfGenerator;

        public ServiceRequestService(IServiceRequestRepository serviceRequestRepository, IMaintenanceServiceRepository maintenanceServiceRepository, IUserRepository userRepository, IOrderServiceReportGenerator pdfGenerator)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _maintenanceServiceRepository = maintenanceServiceRepository;
            _userRepository = userRepository;
            _pdfGenerator = pdfGenerator;
        }

        // Main methods
        public async Task<ResponseUpdateServiceRequest> AcceptServiceRequest(Guid serviceRequestId, Guid technicianId)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");
            serviceRequest.AssignTechnician(technicianId);
            await _serviceRequestRepository.Update(serviceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
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

        public async Task<ResponseUpdateServiceRequest> CancelServiceRequest(Guid serviceRequestId, Guid userId)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId)
                ?? throw new NotFoundException("Solicitação de serviço não encontrada.");
            ValidateServiceRequestOwnership(serviceRequest, userId);
            serviceRequest.Cancel();
            await _serviceRequestRepository.Update(serviceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
        }

        public async Task<ResponseUpdateServiceRequest> FinishServiceRequest(Guid serviceRequestId, Guid technicianId)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId)
                ?? throw new NotFoundException("Solicitação de serviço não encontrada.");
            ValidateServiceRequestTechnicianAssignment(serviceRequest, technicianId);
            serviceRequest.Finish();
            await _serviceRequestRepository.Update(serviceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
        }

        public async Task<Communication.Responses.PagedResult<ResponseGetServiceRequest>> GetServiceRequests(RequestGetServiceRequests filters, Guid userId, bool isQueue)
        {
            filters ??= new RequestGetServiceRequests();

            filters.Page = filters.Page <= 0 ? 1 : filters.Page;
            filters.PageSize = filters.PageSize <= 0 ? 20 : Math.Min(filters.PageSize, 100);
            filters.SortBy = filters.SortBy?.Trim().ToLower() ?? "createdat";
            filters.SortDirection = filters.SortDirection?.Trim().ToLower() ?? "desc";

            var repositoryFilters = ServiceRequestMapper.MapToDomainFilters(filters);

            var serviceRequests = await _serviceRequestRepository.GetAll(repositoryFilters, userId, isQueue);
            return ServiceRequestMapper.ToResponseGetServiceRequests(serviceRequests);
        }

        public async Task<ResponseUpdateServiceRequest> RejectServiceRequest(Guid serviceRequestId, Guid technicianId)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");
            ValidateServiceRequestTechnicianAssignment(serviceRequest, technicianId);
            serviceRequest.Reject();
            await _serviceRequestRepository.Update(serviceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
        }

        public async Task<ResponseUpdateServiceRequest> UpdateServiceRequest(Guid serviceRequestId, RequestUpdateServiceRequest request, Guid technicianId)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId)
            ?? throw new NotFoundException("Solicitação de serviço não encontrada.");
            ValidateServiceRequestTechnicianAssignment(serviceRequest, technicianId);

            serviceRequest.SetTechnicalData(request.TechnicalDiagnosis, request.LaborCost, request.PartsCost);
            await _serviceRequestRepository.Update(serviceRequest);
            return ServiceRequestMapper.ToResponseUpdateServiceRequest(serviceRequest);
        }

        public async Task<ResponseGetServiceRequest> GetServiceRequestById(Guid serviceRequestId, Guid userId, bool isTechnician)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");

            bool isOwner = serviceRequest.UserId == userId;
            bool isAssignedTechnician = serviceRequest.TechnicianId == userId;
            bool isAvailableInQueue = serviceRequest.TechnicianId == null && isTechnician;

            if (!isOwner && !isAssignedTechnician && !isAvailableInQueue)
            {
                throw new NotFoundException("Solicitação de serviço não encontrada.");
            }
            return ServiceRequestMapper.ToResponseGetServiceRequest(serviceRequest);
        }

        public async Task<List<ResponseServiceRequestHistory>> GetTimeServiceRequestTimeline(Guid serviceRequestId, Guid userId)
        {
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");
            bool isOwner = serviceRequest.UserId == userId;
            bool isAssignedTechnician = serviceRequest.TechnicianId == userId;
            if (!isOwner && !isAssignedTechnician)
            {
                throw new NotFoundException("Solicitação de serviço não encontrada.");
            }
            var history = await _serviceRequestRepository.GetHistoryByServiceRequestId(serviceRequestId);

            return ServiceRequestMapper.ToResponseServiceRequestHistoryList(history);
        }

        public async Task<byte[]> GenerateServiceRequestPdfAsync(Guid serviceRequestId, Guid userId, bool isTechnician)
        {
            var serviceRequestResponse = await GetServiceRequestById(serviceRequestId, userId, isTechnician);

            var maintenanceServices = await _maintenanceServiceRepository.GetByIds(serviceRequestResponse.ServiceIds);
            var customer = await _userRepository.GetById(serviceRequestResponse.UserId) ?? throw new NotFoundException("Usuário não encontrado.");
            var serviceRequestDto = ServiceRequestMapper.ToServiceRequestDto(serviceRequestResponse, maintenanceServices, customer);
            var pdfBytes = _pdfGenerator.GenerateOrderServicePdf(serviceRequestDto);

            return pdfBytes;
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
        private void ValidateServiceRequestTechnicianAssignment(ServiceRequest serviceRequest, Guid technicianId)
        {
            if (serviceRequest.TechnicianId != technicianId)
                throw new NotFoundException("Solicitação de serviço não pertence ao técnico.");
        }
        private void ValidateServiceRequestOwnership(ServiceRequest serviceRequest, Guid userId)
        {
            if (serviceRequest.UserId != userId)
                throw new NotFoundException("Solicitação de serviço não encontrada.");
        }
    }
}
