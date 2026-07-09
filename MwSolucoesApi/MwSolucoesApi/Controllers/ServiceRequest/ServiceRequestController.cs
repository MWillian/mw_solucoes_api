using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.Interfaces;
using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Communication;

namespace MwSolucoes.Api.Controllers.ServiceRequest
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestController : MainController
    {
        private readonly IServiceRequestService _serviceRequestService;
        public ServiceRequestController(IServiceRequestService serviceRequestService, IEmailService emailService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ResponseCreateServiceRequest), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateServiceRequest([FromBody] RequestCreateServiceRequest request)
        {
            Guid userId = GetUserId();
            var createdServiceRequest = await _serviceRequestService.CreateServiceRequest(request, userId);
            return Created(string.Empty, createdServiceRequest);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PagedResult<ResponseGetServiceRequest>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyServiceRequests([FromQuery] RequestGetServiceRequests request)
        {
            Guid userId = GetUserId();
            var serviceRequests = await _serviceRequestService.GetServiceRequests(request, userId, isQueue: false);
            return Ok(serviceRequests);
        }

        [HttpGet("newly")]
        [Authorize(Policy = "Technician")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PagedResult<ResponseGetServiceRequest>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnassignedQueue([FromQuery] RequestGetServiceRequests request)
        {
            Guid userId = GetUserId();

            var serviceRequests = await _serviceRequestService.GetServiceRequests(request, userId, isQueue: true);
            return Ok(serviceRequests);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseGetServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServiceRequestById([FromRoute] Guid id)
        {
            Guid userId = GetUserId();
            var isTechnician = User.IsInRole("Técnico");
            var serviceRequest = await _serviceRequestService.GetServiceRequestById(id, userId, isTechnician);
            return Ok(serviceRequest);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Technician")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseUpdateServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateServiceRequestById([FromRoute] Guid id, [FromBody] RequestUpdateServiceRequest request)
        {
            Guid technicianId = GetUserId();
            var response = await _serviceRequestService.UpdateServiceRequest(id, request, technicianId);
            return Ok(response);
        }

        [HttpPut("{id:guid}/accept")]
        [Authorize(Policy = "Technician")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseUpdateServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> AcceptServiceRequest([FromRoute] Guid id)
        {
            Guid userId = GetUserId();
            var response = await _serviceRequestService.AcceptServiceRequest(id, userId);
            return Ok(response);
        }

        [HttpPut("{id:guid}/reject")]
        [Authorize(Policy = "Technician")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseUpdateServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectServiceRequest([FromRoute] Guid id)
        {
            Guid technicianId = GetUserId();
            var response = await _serviceRequestService.RejectServiceRequest(id, technicianId);
            return Ok(response);
        }

        [HttpPut("{id:guid}/finish")]
        [Authorize(Policy = "Technician")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseUpdateServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> FinishServiceRequest([FromRoute] Guid id)
        {
            Guid technicianId = GetUserId();
            var response = await _serviceRequestService.FinishServiceRequest(id, technicianId);
            return Ok(response);
        }

        [HttpPut("{id:guid}/cancel")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseUpdateServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelServiceRequest([FromRoute] Guid id)
        {
            Guid userId = GetUserId();
            var response = await _serviceRequestService.CancelServiceRequest(id, userId);
            return Ok(response);
        }

        [HttpGet("timeline/{serviceRequestId:Guid}")]
        [Authorize]
        [ProducesResponseType(typeof(List<ResponseServiceRequestHistory>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTimeline([FromRoute] Guid serviceRequestId)
        {
            var timeline = await _serviceRequestService.GetTimeServiceRequestTimeline(serviceRequestId, GetUserId());
            return Ok(timeline);
        }

        [HttpGet("{id:guid}/download-os")]
        [Authorize]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadOrderServicePdf([FromRoute] Guid id)
        {
            Guid userId = GetUserId();
            byte[] pdfBytes = await _serviceRequestService.GenerateServiceRequestPdfAsync(id, userId, true);
            return File(pdfBytes, "application/pdf", $"Ordem_Servico_{id}.pdf");
        }

        [HttpGet("{id:guid}/download-receipt")]
        [Authorize] 
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadReceiptPdf([FromRoute] Guid id)
        {
            Guid userId = GetUserId();
            byte[] pdfBytes = await _serviceRequestService.GenerateReceiptPdfAsync(id, userId, false);
            return File(pdfBytes, "application/pdf", $"Recibo_Quitacao_{id}.pdf");
        }

        [HttpPut("{id:guid}/approve-budget")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ApproveBudget([FromRoute] Guid id)
        {
            Guid userId = GetUserId();

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "IP Desconhecido";
            string userAgent = Request.Headers.UserAgent.ToString();

            await _serviceRequestService.ApproveBudgetAsync(id, userId, ipAddress, userAgent);

            return NoContent();
        }

        [HttpPut("{serviceRequestId:guid}/send-os")]
        [Authorize(Policy = "Technician")]
        public async Task<IActionResult> SendOrderServiceToClientEmail([FromRoute] Guid serviceRequestId)
        {
            Guid userId = GetUserId();
            var isTechnician = User.IsInRole("Técnico");
            await _serviceRequestService.SendOrderServiceProposalEmailAsync(serviceRequestId, userId, isTechnician);
            return NoContent();
        }

        [HttpPut("{serviceRequestId:guid}/send-receipt")]
        [Authorize(Policy = "Technician")]
        public async Task<IActionResult> SendReceiptOfFullDischargeToClientEmail([FromRoute] Guid serviceRequestId)
        {
            Guid userId = GetUserId();
            var isTechnician = User.IsInRole("Técnico");
            await _serviceRequestService.SendOrderServiceReceiptAsync(serviceRequestId, userId, isTechnician);
            return NoContent();
        }
    }
}