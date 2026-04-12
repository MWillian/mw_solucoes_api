using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses.MaintenanceService;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.MaintenanceService.Update
{
    public class UpdateMaintenanceServiceUseCase : IUpdateMaintenanceServiceUseCase
    {
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;

        public UpdateMaintenanceServiceUseCase(IMaintenanceServiceRepository maintenanceServiceRepository)
        {
            _maintenanceServiceRepository = maintenanceServiceRepository;
        }

        public async Task<ResponseUpdateMaintenanceService> Execute(int id, RequestUpdateMaintenanceService request)
        {
            if (id <= 0)
                throw new ErrorOnValidationException("O id do serviço de manutenção é inválido.");

            if (request == null)
                throw new ErrorOnValidationException("O objeto de requisição não pode ser nulo.");

            var service = await _maintenanceServiceRepository.GetById(id)
                ?? throw new NotFoundException("Serviço de manutenção não encontrado.");

            var normalizedName = request.Name.Trim();
            var existingServiceWithSameName = await _maintenanceServiceRepository.GetByName(normalizedName);
            if (existingServiceWithSameName is not null && existingServiceWithSameName.Id != service.Id)
                throw new RequestConflictException($"Já existe um serviço de manutenção com o nome '{request.Name}'.");

            service.UpdateFields(request.Name, request.Description, request.Price, request.Category);

            await _maintenanceServiceRepository.Update(service);

            return MaintenanceServiceMapper.ToResponseUpdateMaintenanceService(service);
        }
    }
}
