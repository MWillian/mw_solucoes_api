using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Domain.Entities
{
    public class ServiceRequestItem
    {
        public Guid ServiceRequestId { get; private set; }
        public int MaintenanceServiceId { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }

        public ServiceRequest ServiceRequest { get; private set; } = null!;
        public MaintenanceService MaintenanceService { get; private set; } = null!;

        private ServiceRequestItem() { }

        public ServiceRequestItem(int maintenanceServiceId, decimal unitPrice, int quantity = 1)
        {
            ValidateMaintenanceServiceId(maintenanceServiceId);
            ValidateUnitPrice(unitPrice);
            ValidateQuantity(quantity);

            MaintenanceServiceId = maintenanceServiceId;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        private void ValidateMaintenanceServiceId(int maintenanceServiceId)
        {
            if (maintenanceServiceId <= 0)
                throw new DomainException("O id do serviço de manutenção é inválido.");
        }

        private void ValidateUnitPrice(decimal unitPrice)
        {
            if (unitPrice < 0)
                throw new DomainException("O valor unitário do serviço não pode ser negativo.");
        }

        private void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("A quantidade do serviço deve ser maior que zero.");
        }
    }
}
