using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.MaintenanceService.Delete
{
    public class DeleteMaintenanceServiceUseCase : IDeleteMaintenanceServiceUseCase
    {
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;

        public DeleteMaintenanceServiceUseCase(IMaintenanceServiceRepository maintenanceServiceRepository)
        {
            _maintenanceServiceRepository = maintenanceServiceRepository;
        }

        public async Task Execute(int id)
        {
            if (id <= 0)
                throw new ErrorOnValidationException("O id do serviço de manutenção é inválido.");

            var service = await _maintenanceServiceRepository.GetById(id)
                ?? throw new NotFoundException("Serviço de manutenção não encontrado.");

            await _maintenanceServiceRepository.Delete(service);
        }
    }
}
