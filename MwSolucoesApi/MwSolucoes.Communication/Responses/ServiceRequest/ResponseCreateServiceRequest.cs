namespace MwSolucoes.Communication.Responses.ServiceRequest
{
    public class ResponseCreateServiceRequest
    {
        public Guid Id { get; set; }
        public string Protocol { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int EquipmentType { get; set; }
        public string BrandModel { get; set; } = string.Empty;
        public string ReportedProblem { get; set; } = string.Empty;
        public string? TechnicalDiagnosis { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? PartsCost { get; set; }
        public bool RequiresDownPayment { get; set; }
        public List<int> ServiceIds { get; set; } = [];
    }
}
