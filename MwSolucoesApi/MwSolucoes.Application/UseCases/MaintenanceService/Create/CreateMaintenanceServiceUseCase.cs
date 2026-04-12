using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses.MaintenanceService;
using MwSolucoes.Application.Mappers;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.MaintenanceService.Create
{
    public class CreateMaintenanceServiceUseCase : ICreateMaintenanceServiceUseCase
    {
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;
        public CreateMaintenanceServiceUseCase(IMaintenanceServiceRepository maintenanceServiceRepository)
        {
            _maintenanceServiceRepository = maintenanceServiceRepository;
        }
        public async Task<ResponseCreateMaintenanceService> Execute(RequestCreateMaintenanceService request)
        {
            await ValidateRequest(request);

            var maintenanceService = MaintenanceServiceMapper.ToMaintenanceService(request);
            await _maintenanceServiceRepository.Add(maintenanceService);

            return MaintenanceServiceMapper.ToResponseCreateMaintenanceService(maintenanceService);
        }
        private async Task ValidateRequest(RequestCreateMaintenanceService request)
        {
            if (request == null)
                throw new ErrorOnValidationException("O objeto de requisição não pode ser nulo.");

            var normalizedName = request.Name.Trim();
            var existingService = await _maintenanceServiceRepository.GetByName(normalizedName);
            if (existingService is not null)
                throw new RequestConflictException($"Já existe um serviço de manutenção com o nome '{request.Name}'.");
        }
    }
}
