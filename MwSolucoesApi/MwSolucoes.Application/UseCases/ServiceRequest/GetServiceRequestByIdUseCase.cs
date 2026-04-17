using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.ServiceRequest
{
    public class GetServiceRequestByIdUseCase : IGetServiceRequestByIdUseCase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        public GetServiceRequestByIdUseCase(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }
        public async Task<ResponseGetServiceRequest> Execute(Guid serviceRequestId, Guid userId, bool canViewAll)
        {
            ValidateGuid(serviceRequestId);
            var serviceRequest = await _serviceRequestRepository.GetById(serviceRequestId) ?? throw new NotFoundException("Solicitação de serviço não encontrada.");

            if (!canViewAll && serviceRequest.UserId != userId)
                throw new NotFoundException("Solicitação de serviço não encontrada.");

            return ServiceRequestMapper.ToResponseGetServiceRequest(serviceRequest);
        }
        private void ValidateGuid(Guid serviceRequestId)
        {
            if (serviceRequestId == Guid.Empty)
                throw new ErrorOnValidationException("O ID da solicitação de serviço é obrigatório.");
        }
    }
}
