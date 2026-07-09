using MwSolucoes.Domain.Enums;
using MwSolucoes.Exception.ExceptionBase;
using System.Diagnostics;
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
        public string TechnicalDiagnosis { get; private set; }
        public decimal? PartsCost { get; private set; }
        public bool? RequiresDownPayment { get; private set; }
        public Guid? TechnicianId { get; private set; }
        public User? Technician { get; private set; }
        public DateTime? AcceptedAt { get; set; }
        public string? AcceptedIpAddress { get; set; }
        public string? AcceptedUserAgent { get; set; }
        public User User { get; private set; } = null!;
        private readonly List<ServiceRequestHistory> _histories = [];
        public IReadOnlyCollection<ServiceRequestHistory> Histories => _histories.AsReadOnly();
        private ServiceRequest() { }

        public ServiceRequest(Guid userId, EquipmentType equipmentType, string brandModel, string reportedProblem)
        {
            ValidateUserId(userId);
            ValidateEquipmentType(equipmentType);
            ValidateBrandModel(brandModel);
            ValidateReportedProblem(reportedProblem);

            Id = Guid.NewGuid();
            Protocol = GenerateProtocol();
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            EquipmentType = equipmentType;
            BrandModel = brandModel;
            ReportedProblem = reportedProblem;
            Status = ServiceRequestStatus.Created;
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

        private void ValidateNonNegativeValue(decimal? value, string fieldName)
        {
            if (value.HasValue && value.Value < 0)
                throw new DomainException($"{fieldName} não pode ser negativo.");
        }
        public void AssignTechnician(Guid technicianId)
        {
            StartProgress();
            TechnicianId = technicianId;
            Status = ServiceRequestStatus.InProgress;
            var history = new ServiceRequestHistory(this.Id, ServiceRequestHistoryStatus.InProgress, "Técnico assumiu a solicitação de serviço.");
            _histories.Add(history);
        }
        public void ApproveBudget(string ipAddress, string userAgent)
        {
            if (Status != ServiceRequestStatus.InProgress)
            {
                throw new DomainException("O orçamento não pode ser aprovado neste estágio da solicitação.");
            }

            if (AcceptedAt != null)
            {
                throw new DomainException("Este orçamento já foi aprovado eletronicamente.");
            }

            AcceptedAt = DateTime.UtcNow;
            AcceptedIpAddress = ipAddress;
            AcceptedUserAgent = userAgent;

            _histories.Add(new ServiceRequestHistory(this.Id, ServiceRequestHistoryStatus.InProgress, "Cliente aprovou o orçamento eletronicamente."));
        }
        public void StartProgress()
        {
            if (Status != ServiceRequestStatus.Created)
            {
                throw new DomainException("A solicitação de serviço deve estar no status Criado para iniciar o progresso.");
            }
            Status = ServiceRequestStatus.InProgress;
        }

        public void Finish()
        {
            if (Status != ServiceRequestStatus.InProgress)
            {
                throw new DomainException("A solicitação de serviço deve estar no status Em Progresso para ser finalizada.");
            }
            Status = ServiceRequestStatus.Finished;
            _histories.Add(new ServiceRequestHistory(this.Id, ServiceRequestHistoryStatus.Finished, "Serviço finalizado com sucesso."));
        }

        public void Cancel()
        {
            if (Status != ServiceRequestStatus.Created)
            {
                throw new DomainException("A solicitação de serviço deve estar no status Criada para ser Cancelada.");
            }
            Status = ServiceRequestStatus.Canceled;
            _histories.Add(new ServiceRequestHistory(this.Id, ServiceRequestHistoryStatus.Canceled, "Serviço cancelado com sucesso."));
        }

        public void Reject()
        {
            if (Status != ServiceRequestStatus.Created)
            {
                throw new DomainException("A solicitação de serviço deve estar no status Criado para ser rejeitada.");
            }
            Status = ServiceRequestStatus.Rejected;
            _histories.Add(new ServiceRequestHistory(this.Id, ServiceRequestHistoryStatus.Rejected, "O Serviço foi rejeitado."));
        }

        public void SetTechnicalData(string technicalDiagnosis, decimal? partsCost)
        {
            ValidateNonNegativeValue(partsCost, "Valor de peças");

            TechnicalDiagnosis = technicalDiagnosis;
            PartsCost = partsCost;
        }
        public void UpdateItems(List<ServiceRequestItem> incomingItems)
        {
            if (Status != ServiceRequestStatus.Created && Status != ServiceRequestStatus.InProgress)
            {
                throw new DomainException("Os serviços só podem ser alterados durante a criação ou análise técnica.");
            }

            if (incomingItems == null || incomingItems.Count == 0)
            {
                throw new DomainException("Para gerar o orçamento, insira ao menos um serviço.");
            }

            Items ??= new List<ServiceRequestItem>();

            var itemsToRemove = Items
                .Where(existing => !incomingItems.Any(incoming => incoming.MaintenanceServiceId == existing.MaintenanceServiceId))
                .ToList();

            var itemsToAdd = incomingItems
                .Where(incoming => !Items.Any(existing => existing.MaintenanceServiceId == incoming.MaintenanceServiceId))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                Items.Remove(item);
            }

            foreach (var item in itemsToAdd)
            {
                Items.Add(item);
            }
        }
    }
}
