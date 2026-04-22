using MwSolucoes.Domain.Enums;
using MwSolucoes.Exception.ExceptionBase;
using System.Security.Cryptography;

namespace MwSolucoes.Domain.Entities
{
    public class ServiceRequest
    {
        public Guid Id { get; private set; }
        public List<ServiceRequestItem> Items { get; private set; } = [];
        public string Protocol { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public ServiceRequestStatus Status { get; private set; } = ServiceRequestStatus.Created;
        public DateTime CreatedAt { get; private set; }
        public EquipmentType EquipmentType { get; private set; }
        public string BrandModel { get; private set; } = string.Empty;
        public string ReportedProblem { get; private set; } = string.Empty;
        public string? TechnicalDiagnosis { get; private set; }
        public decimal? LaborCost { get; private set; }
        public decimal? PartsCost { get; private set; }
        public bool RequiresDownPayment { get; private set; }

        public User User { get; private set; } = null!;
        public List<ServiceRequestHistory> Histories { get; private set; } = [];

        private ServiceRequest() { }

        public ServiceRequest(Guid userId, EquipmentType equipmentType, string brandModel, string reportedProblem, bool requiresDownPayment, IEnumerable<ServiceRequestItem> items)
        {
            ValidateUserId(userId);
            ValidateEquipmentType(equipmentType);
            ValidateBrandModel(brandModel);
            ValidateReportedProblem(reportedProblem);
            ValidateItems(items);

            Id = Guid.NewGuid();
            Protocol = GenerateProtocol();
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            EquipmentType = equipmentType;
            BrandModel = brandModel;
            ReportedProblem = reportedProblem;
            RequiresDownPayment = requiresDownPayment;
            Status = ServiceRequestStatus.Created;
            Items = items
                .DistinctBy(item => item.MaintenanceServiceId)
                .ToList();
        }

        private static string GenerateProtocol()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = RandomNumberGenerator.GetInt32(0, 1000).ToString("D3");
            return $"{datePart}{randomPart}";
        }

        private void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("O usuário da solicitação é obrigatório.");
        }

        private void ValidateEquipmentType(EquipmentType equipmentType)
        {
            if (!Enum.IsDefined(typeof(EquipmentType), equipmentType))
                throw new DomainException("Tipo de equipamento inválido.");
        }

        private void ValidateBrandModel(string brandModel)
        {
            if (string.IsNullOrWhiteSpace(brandModel))
                throw new DomainException("Marca/Modelo é obrigatório.");

            if (brandModel.Length > 100)
                throw new DomainException("Marca/Modelo deve ter no máximo 100 caracteres.");
        }

        private void ValidateReportedProblem(string reportedProblem)
        {
            if (string.IsNullOrWhiteSpace(reportedProblem))
                throw new DomainException("Problema relatado é obrigatório.");
        }

        private void ValidateItems(IEnumerable<ServiceRequestItem> items)
        {
            if (items is null || !items.Any())
                throw new DomainException("A solicitação deve conter ao menos um serviço.");
        }

        private void ValidateNonNegativeValue(decimal? value, string fieldName)
        {
            if (value.HasValue && value.Value < 0)
                throw new DomainException($"{fieldName} não pode ser negativo.");
        }

        public void StartProgress()
        {
            Status = ServiceRequestStatus.InProgress;
        }

        public void Finish()
        {
            Status = ServiceRequestStatus.Finished;
        }

        public void Cancel()
        {
            Status = ServiceRequestStatus.Canceled;
        }

        public void Reject()
        {
            Status = ServiceRequestStatus.Rejected;
        }

        public void SetTechnicalData(string? technicalDiagnosis, decimal? laborCost, decimal? partsCost)
        {
            ValidateNonNegativeValue(laborCost, "Valor de mão de obra");
            ValidateNonNegativeValue(partsCost, "Valor de peças");

            TechnicalDiagnosis = technicalDiagnosis;
            LaborCost = laborCost;
            PartsCost = partsCost;
        }
    }
}
