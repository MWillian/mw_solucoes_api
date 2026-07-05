using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.Interfaces;
using MwSolucoes.Communication.Requests.ServiceRequestHistory;
using MwSolucoes.Domain.Repositories;

namespace MwSolucoes.Api.Controllers.ServiceRequestHistory
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestHistoryController : MainController
    {
        private readonly IServiceRequestService _serviceRequestService;
        public ServiceRequestHistoryController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        //[HttpPut]
        //[Authorize]
        //public async Task<IActionResult> UpdateHistory([FromBody] RequestUpdateServiceRequestHistory request)
        //{
        //    var serviceRequest = await _serviceRequestRepository.GetByIdAsync(request.ServiceRequestId);
        //    if (serviceRequest == null)
        //        return NotFound("Solicitação de serviço não encontrada.");
        //    serviceRequest.UpdateStatus(request.Status, request.Description);
        //    await _serviceRequestHistoryRepository.UpdateHistory(serviceRequest);
        //    return NoContent();
        //}
}
}
