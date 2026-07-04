using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.Interfaces;
using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.ServiceRequest;
using MwSolucoes.Domain.Enums;
using System.Security.Claims;

namespace MwSolucoes.Api.Controllers.ServiceRequest
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestController : ControllerBase
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var createdServiceRequest = await _serviceRequestService.CreateServiceRequest(request, userId);
            return Created(string.Empty, createdServiceRequest);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PagedResult<ResponseGetServiceRequest>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServiceRequests([FromQuery] RequestGetServiceRequests request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
            var canViewAll = User.IsInRole(UserRoles.Técnico.ToString());
            var serviceRequests = await _serviceRequestService.GetServiceRequests(request, userId, canViewAll);
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
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
        public async Task<IActionResult> UpdateServiceRequestById([FromRoute] Guid id, [FromBody] RequestUpdateServiceRequest request)
        {
            var response = await _serviceRequestService.UpdateServiceRequest(id, request);
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
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
            var response = await _serviceRequestService.AcceptServiceRequest(id);
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
            var response = await _serviceRequestService.CancelServiceRequest(id);
            return Ok(response);
        }
    }
}