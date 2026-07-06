using MwSolucoes.Domain.Enums;

namespace MwSolucoes.Communication.Responses.ServiceRequest
{
    public class ResponseServiceRequestHistory
    {
        public ServiceRequestHistoryStatus Status { get; set; }
        public string Description = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
