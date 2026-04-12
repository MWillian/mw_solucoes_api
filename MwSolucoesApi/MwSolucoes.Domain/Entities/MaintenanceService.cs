using MwSolucoes.Domain.Enums;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Domain.Entities
{
    public class MaintenanceService
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; } = true;
        public MaintenanceServiceCategories Category { get; private set; }
        private MaintenanceService() { }
        public MaintenanceService(string name, string description, decimal price, MaintenanceServiceCategories category)
        {
            ValidateFields(name, description, price, category);
            Name = name;
            Description = description;
            Price = price;
            Category = category;
        }
        private void ValidateFields(string name, string description, decimal price, MaintenanceServiceCategories category)
        {
            ValidateName(name);
            ValidateDescription(description);
            ValidatePrice(price);
            ValidateCategory(category);
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome do serviço não pode ser vazio.");
            if (name.Length < 10 || name.Length > 50)
                throw new DomainException("O nome do serviço deve ter entre 10 e 50 caracteres.");
        }
        private void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("A descrição do serviço não pode ser vazia.");
            if (description.Length < 10 || description.Length > 200)
                throw new DomainException("A descrição do serviço deve ter entre 10 e 200 caracteres. ");
        }
        private void ValidatePrice(decimal price)
        {
            if (price <= 0)
                throw new DomainException("O preço do serviço precisa ser maior do que 0.");

        }
        private void ValidateCategory(MaintenanceServiceCategories category)
        {
            if (!Enum.IsDefined(typeof(MaintenanceServiceCategories), category))
                throw new DomainException("Categoria de serviço inválida.");
        }

        //Helper Methods

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("O serviço já está ativo.");
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("O serviço já está inativo.");
            IsActive = false;
        }

        public void UpdateFields(string name, string description, decimal price, MaintenanceServiceCategories category)
        {
            ValidateFields(name, description, price, category);
            Name = name;
            Description = description;
            Price = price;
            Category = category;
        }
    }
}
