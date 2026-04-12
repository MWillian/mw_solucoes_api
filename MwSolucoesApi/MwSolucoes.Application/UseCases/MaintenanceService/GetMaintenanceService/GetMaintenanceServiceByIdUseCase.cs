using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Responses.MaintenanceService;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.MaintenanceService.GetMaintenanceService
{
    public class GetMaintenanceServiceByIdUseCase : IGetMaintenanceServiceByIdUseCase
    {
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;

        public GetMaintenanceServiceByIdUseCase(IMaintenanceServiceRepository maintenanceServiceRepository)
        {
            _maintenanceServiceRepository = maintenanceServiceRepository;
        }

        public async Task<ResponseGetMaintenanceService> Execute(int id)
        {
            if (id <= 0)
                throw new ErrorOnValidationException("O id do serviço de manutenção é inválido.");

            var service = await _maintenanceServiceRepository.GetById(id)
                ?? throw new NotFoundException("Serviço de manutenção não encontrado.");

            return MaintenanceServiceMapper.ToResponseGetMaintenanceService(service);
        }
    }
}
