using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.MaintenanceService.Deactivate
{
    public class DeactivateMaintenanceServiceUseCase : IDeactivateMaintenanceServiceUseCase
    {
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;

        public DeactivateMaintenanceServiceUseCase(IMaintenanceServiceRepository maintenanceServiceRepository)
        {
            _maintenanceServiceRepository = maintenanceServiceRepository;
        }

        public async Task Execute(int id)
        {
            if (id <= 0)
                throw new ErrorOnValidationException("O id do serviço de manutenção é inválido.");

            var service = await _maintenanceServiceRepository.GetById(id)
                ?? throw new NotFoundException("Serviço de manutenção não encontrado.");

            service.Deactivate();
            await _maintenanceServiceRepository.Update(service);
        }
    }
}
