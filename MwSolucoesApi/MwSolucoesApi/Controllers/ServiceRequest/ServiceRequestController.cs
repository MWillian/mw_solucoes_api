using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.Interfaces;
using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Enums;

namespace MwSolucoes.Api.Controllers.ServiceRequest
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestController : MainController
    {
        private readonly IServiceRequestService _serviceRequestService;
        public ServiceRequestController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpPost()]
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

        [Authorize]
        [HttpGet("newly")]
        [Authorize(Roles = "Técnico")]
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
        public async Task<IActionResult> GetServiceRequestsById([FromRoute] Guid id)
        {
            Guid userId = GetUserId();

            var canViewAll = User.IsInRole(UserRoles.Técnico.ToString());
            var serviceRequest = await _serviceRequestService.GetServiceRequestById(id, userId, canViewAll);
            return Ok(serviceRequest);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseUpdateServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateServiceRequestById([FromRoute] Guid serviceRequestId, [FromBody] RequestUpdateServiceRequest request)
        {
            Guid _ = GetUserId();
            var response = await _serviceRequestService.UpdateServiceRequest(serviceRequestId, request);
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteServiceRequestById([FromRoute] Guid id)
        {
            Guid userId = GetUserId();
            var canViewAll = User.IsInRole(UserRoles.Técnico.ToString());
            await _serviceRequestService.DeleteServiceRequest(id, userId, canViewAll);
            return NoContent();
        }

        [HttpPut("{id:guid}/accept")]
        [Authorize(Roles = "Técnico")]
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
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseUpdateServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectServiceRequest([FromRoute] Guid id)
        {
            Guid _ = GetUserId();
            var response = await _serviceRequestService.RejectServiceRequest(id);
            return Ok(response);
        }

        [HttpPut("{id:guid}/finish")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseUpdateServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> FinishServiceRequest([FromRoute] Guid id)
        {
            Guid _ = GetUserId();
            var response = await _serviceRequestService.FinishServiceRequest(id);
            return Ok(response);
        }

        [HttpPut("{id:guid}/cancel")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseUpdateServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelServiceRequest([FromRoute] Guid id)
        {
            Guid _ = GetUserId();
            var response = await _serviceRequestService.CancelServiceRequest(id);
            return Ok(response);
        }
    }
}