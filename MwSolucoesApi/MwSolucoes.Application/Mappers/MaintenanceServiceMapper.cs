using MwSolucoes.Domain.Entities;
using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses.MaintenanceService;
using DomainMaintenanceServiceFilters = MwSolucoes.Domain.Repositories.Filters.MaintenanceServiceFilters;

namespace MwSolucoes.Application.Mappers
{
    public static class MaintenanceServiceMapper
    {
        public static MaintenanceService ToMaintenanceService(RequestCreateMaintenanceService request)
        {
            return new MaintenanceService(request.Name, request.Description, request.Price, request.Category);
        }

        public static ResponseCreateMaintenanceService ToResponseCreateMaintenanceService(MaintenanceService service)
        {
            return new ResponseCreateMaintenanceService
            {
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                Category = service.Category
            };
        }

        public static ResponseUpdateMaintenanceService ToResponseUpdateMaintenanceService(MaintenanceService service)
        {
            return new ResponseUpdateMaintenanceService
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                IsActive = service.IsActive,
                Category = service.Category
            };
        }

        public static ResponseGetMaintenanceService ToResponseGetMaintenanceService(MaintenanceService service)
        {
            return new ResponseGetMaintenanceService
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                IsActive = service.IsActive,
                Category = service.Category
            };
        }

        public static Communication.Responses.PagedResult<ResponseGetMaintenanceService> ToResponseGetMaintenanceServices(Domain.Entities.PagedResult<MaintenanceService> services)
        {
            var responseItems = services.Items.Select(ToResponseGetMaintenanceService).ToList();
            return new Communication.Responses.PagedResult<ResponseGetMaintenanceService>(responseItems, services.TotalCount, services.CurrentPage, services.PageSize);
        }

        public static DomainMaintenanceServiceFilters ToDomainFilters(MaintenanceServiceFilters filters)
        {
            filters ??= new MaintenanceServiceFilters();

            filters.Page = filters.Page <= 0 ? 1 : filters.Page;
            filters.PageSize = filters.PageSize <= 0 ? 20 : Math.Min(filters.PageSize, 100);
            filters.SortBy = string.IsNullOrWhiteSpace(filters.SortBy) ? "name" : filters.SortBy;
            filters.SortDirection = string.IsNullOrWhiteSpace(filters.SortDirection) ? "asc" : filters.SortDirection;

            return new DomainMaintenanceServiceFilters
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
        }
    }
}
