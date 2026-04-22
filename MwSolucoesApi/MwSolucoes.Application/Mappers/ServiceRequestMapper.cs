using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Enums;

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
                ServiceIds = serviceRequest.Items.Select(item => item.MaintenanceServiceId).ToList()
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
    }
}
