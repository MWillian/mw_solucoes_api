using MwSolucoes.Domain.Entities;
using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses.MaintenanceService;

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
    }
}
