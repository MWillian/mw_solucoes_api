using MwSolucoes.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.MaintenanceService
{
    public class RequestCreateMaintenanceService
    {
        [Required(ErrorMessage = "Nome do serviço está vazio.")]
        [StringLength(50, MinimumLength = 7, ErrorMessage = "O nome do serviço deve ter entre 10 e 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição do serviço está vazia.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "A descrição do serviço deve ter entre 10 e 200 caracteres.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preço serviço está vazio.")]
        [Range(1, double.MaxValue, ErrorMessage = "O preço do serviço deve ser maior do que 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Categoria do serviço está vazia.")]
        public MaintenanceServiceCategories Category { get; set; }
    }
}
