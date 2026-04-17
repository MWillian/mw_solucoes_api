using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public class DeleteServiceRequestUseCase : IDeleteServiceRequestUseCase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        public DeleteServiceRequestUseCase(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }
        public async Task Execute(Guid serviceRequestId, Guid userId, bool canViewAll)
        {
            ValidateGuid(serviceRequestId);
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");

            if (!canViewAll && serviceRequest.UserId != userId)
                throw new NotFoundException("Solicitação de serviço não encontrada.");

            await _serviceRequestRepository.DeleteById(serviceRequestId);
        }
        private void ValidateGuid(Guid serviceRequestId)
        {
            if (serviceRequestId == Guid.Empty)
                throw new ErrorOnValidationException("O ID da solicitação de serviço é obrigatório.");
        }
    }
}
