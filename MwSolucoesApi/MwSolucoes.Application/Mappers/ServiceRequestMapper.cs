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
                RequiresDownPayment = serviceRequest.RequiresDownPayment
            };
        }
        public static ServiceRequest ToServiceRequest(RequestCreateServiceRequest request, Guid userId, string ?technicalDiagnosis, decimal laborCost, decimal partsCost)
        {
            var newUser = new ServiceRequest(
                userId,
                (EquipmentType)request.EquipmentType,
                request.BrandModel,
                request.ReportedProblem,
                request.RequiresDownPayment
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
