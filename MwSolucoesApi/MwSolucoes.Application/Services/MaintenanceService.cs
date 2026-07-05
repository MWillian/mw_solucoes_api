using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Interfaces;
using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.MaintenanceService;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;
        public MaintenanceService(IMaintenanceServiceRepository maintenanceServiceRepository)
        {
            _maintenanceServiceRepository = maintenanceServiceRepository;
        }
        public async Task<ResponseCreateMaintenanceService> CreateMaintenanceService(RequestCreateMaintenanceService request)
        {
            await ValidateRequest(request);

            var maintenanceService = MaintenanceServiceMapper.ToMaintenanceService(request);
            await _maintenanceServiceRepository.Add(maintenanceService);

            return MaintenanceServiceMapper.ToResponseCreateMaintenanceService(maintenanceService);
        }

        public async Task DeactivateMaintenanceService(int id)
        {
            var service = await ValidadeIdAndService(id);
            service.Deactivate();
            await _maintenanceServiceRepository.Update(service);
        }

        public async Task DeleteMaintenanceService(int id)
        {
            var service = await ValidadeIdAndService(id);
            await _maintenanceServiceRepository.Delete(service);
        }

        public async Task<ResponseGetMaintenanceService> GetMaintenanceServiceById(int id, bool isTechnician)
        {
            Domain.Entities.MaintenanceService? service;
            if (!isTechnician)
            {
                service = await _maintenanceServiceRepository.GetActiveById(id) ?? throw new NotFoundException("Serviço de manutenção não encontrado.");
            }
            else
            {
                service = await ValidadeIdAndService(id);
            }
            return MaintenanceServiceMapper.ToResponseGetMaintenanceService(service);
        }

        public async Task<PagedResult<ResponseGetMaintenanceService>> GetMaintenanceServices(MaintenanceServiceFilters filters, bool isTechnician)
        {
            var repositoryFilters = MaintenanceServiceMapper.ToDomainFilters(filters);
            if (!isTechnician)
            {
                repositoryFilters.IsActive = true; 
            }
            var services = await _maintenanceServiceRepository.GetAll(repositoryFilters);
            return MaintenanceServiceMapper.ToResponseGetMaintenanceServices(services);
        }

        public async Task Reactivate(int id)
        {
            var service = await ValidadeIdAndService(id);
            service.Activate();
            await _maintenanceServiceRepository.Update(service);
        }

        public async Task<ResponseUpdateMaintenanceService> UpdateMaintenanceService(int id, RequestUpdateMaintenanceService request)
        {
            var service = await ValidadeIdAndService(id);

            await ValidateRequest(request);

            service.UpdateFields(request.Name, request.Description, request.Price, request.Category);

            await _maintenanceServiceRepository.Update(service);

            return MaintenanceServiceMapper.ToResponseUpdateMaintenanceService(service);
        }

        // Helper methods
        private async Task ValidateRequest(IUniqueMaintenanceServiceData request)
        {
            var normalizedName = request.Name.Trim();
            var existingService = await _maintenanceServiceRepository.GetByName(normalizedName);
            if (existingService is not null)
                throw new RequestConflictException("Já existe um serviço de manutenção com este nome.");
        }

        private async Task<Domain.Entities.MaintenanceService> ValidadeIdAndService(int id)
        {
            if (id <= 0) throw new ErrorOnValidationException("O id do serviço de manutenção é inválido.");
            var service = await _maintenanceServiceRepository.GetById(id);
            return service is null ? throw new NotFoundException("Serviço de manutenção não encontrado.") : service;
        }
    }
}
