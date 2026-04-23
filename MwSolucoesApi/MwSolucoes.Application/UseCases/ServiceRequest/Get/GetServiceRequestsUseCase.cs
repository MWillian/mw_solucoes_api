using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Repositories;
using DomainServiceRequestFilters = MwSolucoes.Domain.Repositories.Filters.ServiceRequestFilters;

namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public class GetServiceRequestsUseCase : IGetServiceRequestsUseCase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public GetServiceRequestsUseCase(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<PagedResult<ResponseGetServiceRequest>> Execute(RequestGetServiceRequests filters, Guid userId, bool canViewAll)
        {
            filters ??= new RequestGetServiceRequests();

            filters.Page = filters.Page <= 0 ? 1 : filters.Page;
            filters.PageSize = filters.PageSize <= 0 ? 20 : Math.Min(filters.PageSize, 100);
            filters.SortBy = string.IsNullOrWhiteSpace(filters.SortBy) ? "createdAt" : filters.SortBy;
            filters.SortDirection = string.IsNullOrWhiteSpace(filters.SortDirection) ? "desc" : filters.SortDirection;

            var repositoryFilters = new DomainServiceRequestFilters
            {
                Status = filters.Status,
                CreatedAt = filters.CreatedAt,
                Protocol = filters.Protocol,
                EquipmentType = filters.EquipmentType,
                LaborCost = filters.LaborCost,
                PartsCost = filters.PartsCost,
                Page = filters.Page,
                PageSize = filters.PageSize,
                SortBy = filters.SortBy,
                SortDirection = filters.SortDirection
            };

            Guid? scopedUserId = canViewAll ? null : userId;
            var serviceRequests = await _serviceRequestRepository.GetAll(repositoryFilters, scopedUserId);
            return ServiceRequestMapper.ToResponseGetServiceRequests(serviceRequests);
        }
    }
}
