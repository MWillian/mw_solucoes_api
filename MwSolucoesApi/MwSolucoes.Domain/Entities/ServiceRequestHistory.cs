using MwSolucoes.Domain.Enums;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Domain.Entities
{
    public class ServiceRequestHistory
    {
        private ServiceRequestHistory() { }
        public int Id { get; private set; }
        public Guid ServiceRequestId { get; private set; }
        public ServiceRequestHistoryStatus Status { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        public ServiceRequest ServiceRequest { get; private set; } = null!;

        public ServiceRequestHistory(Guid serviceRequestId, ServiceRequestHistoryStatus status, string description)
        {
            ValidateServiceRequestId(serviceRequestId);
            ValidateStatus(status);

            ServiceRequestId = serviceRequestId;
            Status = status;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        private static void ValidateServiceRequestId(Guid serviceRequestId)
        {
            if (serviceRequestId == Guid.Empty)
                throw new DomainException("O id da solicitação de serviço é obrigatório.");
        }

        private static void ValidateStatus(ServiceRequestHistoryStatus status)
        {
            if (!Enum.IsDefined(typeof(ServiceRequestHistoryStatus), status))
                throw new DomainException("Status do histórico da solicitação inválido.");
        }

    }
}
