namespace MwSolucoes.Application.UseCases.MaintenanceService.Deactivate
{
    public interface IDeactivateMaintenanceServiceUseCase
    {
        Task Execute(int id);
    }
}
