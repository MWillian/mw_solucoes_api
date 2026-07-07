using MwSolucoes.Domain.Enums;

namespace MwSolucoes.Communication.Responses.ServiceRequest
{
    public class ResponseServiceRequestHistory
    {
        public ServiceRequestHistoryStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
