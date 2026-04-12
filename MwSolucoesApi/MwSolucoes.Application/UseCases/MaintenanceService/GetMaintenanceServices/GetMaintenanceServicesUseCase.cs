using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.MaintenanceService;
using MwSolucoes.Domain.Repositories;
using DomainMaintenanceServiceFilters = MwSolucoes.Domain.Repositories.Filters.MaintenanceServiceFilters;

namespace MwSolucoes.Application.UseCases.MaintenanceService.GetMaintenanceServices
{
    public class GetMaintenanceServicesUseCase : IGetMaintenanceServicesUseCase
    {
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;

        public GetMaintenanceServicesUseCase(IMaintenanceServiceRepository maintenanceServiceRepository)
        {
            _maintenanceServiceRepository = maintenanceServiceRepository;
        }

        public async Task<PagedResult<ResponseGetMaintenanceService>> Execute(MaintenanceServiceFilters filters)
        {
            filters ??= new MaintenanceServiceFilters();

            filters.Page = filters.Page <= 0 ? 1 : filters.Page;
            filters.PageSize = filters.PageSize <= 0 ? 20 : Math.Min(filters.PageSize, 100);
            filters.SortBy = string.IsNullOrWhiteSpace(filters.SortBy) ? "name" : filters.SortBy;
            filters.SortDirection = string.IsNullOrWhiteSpace(filters.SortDirection) ? "asc" : filters.SortDirection;

            var repositoryFilters = new DomainMaintenanceServiceFilters
            {
                Name = filters.Name,
                Price = filters.Price,
                IsActive = filters.IsActive,
                Category = filters.Category,
                Page = filters.Page,
                PageSize = filters.PageSize,
                SortBy = filters.SortBy,
                SortDirection = filters.SortDirection
            };

            var services = await _maintenanceServiceRepository.GetAll(repositoryFilters);
            return MaintenanceServiceMapper.ToResponseGetMaintenanceServices(services);
        }
    }
}
