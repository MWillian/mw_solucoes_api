using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.DTOs;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Enums;
using DomainServiceRequestFilters = MwSolucoes.Domain.Repositories.Filters.ServiceRequestFilters;


namespace MwSolucoes.Application.Mappers
{
    public static class ServiceRequestMapper
    {
        public static ResponseCreateServiceRequest ToResponseCreateServiceRequest(ServiceRequest serviceRequest)
        {
            return new ResponseCreateServiceRequest
            {
                Id = serviceRequest.Id,
                Protocol = serviceRequest.Protocol,
                UserId = serviceRequest.UserId,
                Status = (int)serviceRequest.Status,
                CreatedAt = serviceRequest.CreatedAt,
                EquipmentType = (int)serviceRequest.EquipmentType,
                BrandModel = serviceRequest.BrandModel,
                ReportedProblem = serviceRequest.ReportedProblem,
                TechnicalDiagnosis = serviceRequest.TechnicalDiagnosis,
                LaborCost = serviceRequest.LaborCost,
                PartsCost = serviceRequest.PartsCost,
                RequiresDownPayment = serviceRequest.RequiresDownPayment,
                ServiceIds = serviceRequest.Items.Select(item => item.MaintenanceServiceId).ToList()
            };
        }

        public static ResponseGetServiceRequest ToResponseGetServiceRequest(ServiceRequest serviceRequest)
        {
            return new ResponseGetServiceRequest
            {
                Id = serviceRequest.Id,
                Protocol = serviceRequest.Protocol,
                UserId = serviceRequest.UserId,
                Status = serviceRequest.Status,
                CreatedAt = serviceRequest.CreatedAt,
                EquipmentType = serviceRequest.EquipmentType,
                BrandModel = serviceRequest.BrandModel,
                ReportedProblem = serviceRequest.ReportedProblem,
                TechnicalDiagnosis = serviceRequest.TechnicalDiagnosis,
                LaborCost = serviceRequest.LaborCost,
                PartsCost = serviceRequest.PartsCost,
                RequiresDownPayment = serviceRequest.RequiresDownPayment,
                ServiceIds = serviceRequest.Items.Select(item => item.MaintenanceServiceId).ToList(),
                AcceptedAt = serviceRequest.AcceptedAt
            };
        }

        public static Communication.Responses.PagedResult<ResponseGetServiceRequest> ToResponseGetServiceRequests(PagedResult<ServiceRequest> serviceRequests)
        {
            var responseItems = serviceRequests.Items.Select(ToResponseGetServiceRequest).ToList();
            return new Communication.Responses.PagedResult<ResponseGetServiceRequest>(responseItems, serviceRequests.TotalCount, serviceRequests.CurrentPage, serviceRequests.PageSize);
        }

        public static ResponseUpdateServiceRequest ToResponseUpdateServiceRequest(ServiceRequest serviceRequest)
        {
            return new ResponseUpdateServiceRequest
            {
                Id = serviceRequest.Id,
                Protocol = serviceRequest.Protocol,
                UserId = serviceRequest.UserId,
                Status = serviceRequest.Status,
                CreatedAt = serviceRequest.CreatedAt,
                EquipmentType = serviceRequest.EquipmentType,
                BrandModel = serviceRequest.BrandModel,
                ReportedProblem = serviceRequest.ReportedProblem,
                TechnicalDiagnosis = serviceRequest.TechnicalDiagnosis,
                LaborCost = serviceRequest.LaborCost,
                PartsCost = serviceRequest.PartsCost,
                RequiresDownPayment = serviceRequest.RequiresDownPayment,
                ServiceIds = serviceRequest.Items.Select(item => item.MaintenanceServiceId).ToList()
            };
        }

        public static ServiceRequest ToServiceRequest(RequestCreateServiceRequest request, Guid userId, string? technicalDiagnosis, decimal laborCost, decimal partsCost, List<ServiceRequestItem> items)
        {
            var newUser = new ServiceRequest(
                userId,
                (EquipmentType)request.EquipmentType,
                request.BrandModel,
                request.ReportedProblem,
                request.RequiresDownPayment,
                items
            );
            newUser.SetTechnicalData(
                technicalDiagnosis,
                laborCost,
                partsCost
            );
            return newUser;
        }
        public static DomainServiceRequestFilters MapToDomainFilters(RequestGetServiceRequests filters)
        {
            return new DomainServiceRequestFilters
            {
                Status = filters.Status,
                CreatedAt = filters.CreatedAt,
                Protocol = filters.Protocol,
                EquipmentType = filters.EquipmentType,
                LaborCost = filters.LaborCost,
                PartsCost = filters.PartsCost,
                Page = filters.Page,
                PageSize = filters.PageSize,
                SortBy = filters.SortBy,
                SortDirection = filters.SortDirection
            };
        }

        public static List<ResponseServiceRequestHistory> ToResponseServiceRequestHistoryList(List<ServiceRequestHistory> history)
        {
            return new List<ResponseServiceRequestHistory>(history.Select(h => new ResponseServiceRequestHistory
            {
                Status = h.Status,
                Description = h.Description ?? string.Empty,
                CreatedAt = h.CreatedAt
            }));
        }

        public static ServiceRequestReportDto ToServiceRequestDto(ResponseGetServiceRequest serviceRequestResponse, List<MaintenanceService> maintenanceServices, User? user)
        {
            return new ServiceRequestReportDto
            {
                Protocol = serviceRequestResponse.Protocol,
                CreatedAt = serviceRequestResponse.CreatedAt,
                Equipment = serviceRequestResponse.EquipmentType,
                BrandModel = serviceRequestResponse.BrandModel,
                ReportedProblem = serviceRequestResponse.ReportedProblem,
                TechnicalDiagnosis = serviceRequestResponse.TechnicalDiagnosis,
                AcceptedAt = serviceRequestResponse.AcceptedAt,
                LaborCost = serviceRequestResponse.LaborCost,
                PartsCost = serviceRequestResponse.PartsCost,
                CustomerCpf = user.CPF,
                CustomerEmail = user.Email,
                CustomerName = user.Name,
                CustomerPhone = user.PhoneNumber,
                Services = ToMaintenanceServiceItemDtoList(maintenanceServices),
                Status = serviceRequestResponse.Status
            };
        }
        public static List<MaintenanceServiceItemDto> ToMaintenanceServiceItemDtoList(List<MaintenanceService> maintenanceServices)
        {
            return maintenanceServices.Select(service => new MaintenanceServiceItemDto
            {
                Name = service.Name,
                Price = service.Price
            }).ToList();
        }
        public static ReceiptReportDto ToReceiptReportDto(
            ResponseGetServiceRequest serviceRequestResponse,
            List<MaintenanceService> maintenanceServices,
            User user,
            PaymentMethod paymentMethod)
        {
            decimal servicesTotal = maintenanceServices.Sum(s => s.Price);
            decimal? totalAmount = servicesTotal + serviceRequestResponse.LaborCost + serviceRequestResponse.PartsCost;

            return new ReceiptReportDto
            {
                Protocol = serviceRequestResponse.Protocol,
                FinishedAt = DateTime.Now,
                Equipment = serviceRequestResponse.EquipmentType,
                BrandModel = serviceRequestResponse.BrandModel,
                LaborCost = serviceRequestResponse.LaborCost,
                PartsCost = serviceRequestResponse.PartsCost,
                CustomerCpf = user.CPF,
                CustomerName = user.Name,
                Services = ToMaintenanceServiceItemDtoList(maintenanceServices), 
                TotalAmount = totalAmount,
                PaymentMethod = paymentMethod
            };
        }
    }
}
