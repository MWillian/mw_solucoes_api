namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public interface IDeleteServiceRequestUseCase
    {
        Task Execute(Guid serviceRequestId, Guid userId, bool canViewAll);
    }
}
