using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.UseCases.ServiceRequest;
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
        [HttpPost()]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ResponseCreateServiceRequest), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateServiceRequest([FromBody] RequestCreateServiceRequest request, [FromServices] ICreateServiceRequestUseCase useCase)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var createdServiceRequest = await useCase.Execute(request, userId);
            return Created(string.Empty, createdServiceRequest);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PagedResult<ResponseGetServiceRequest>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServiceRequests([FromQuery] RequestGetServiceRequests request, [FromServices] IGetServiceRequestsUseCase useCase)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
            var canViewAll = User.IsInRole(UserRoles.Técnico.ToString());
            var serviceRequests = await useCase.Execute(request, userId, canViewAll);
            return Ok(serviceRequests);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseGetServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServiceRequestsById([FromServices] IGetServiceRequestByIdUseCase useCase, [FromRoute] Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
            var canViewAll = User.IsInRole(UserRoles.Técnico.ToString());
            var serviceRequest = await useCase.Execute(id, userId, canViewAll);
            return Ok(serviceRequest);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseUpdateServiceRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateServiceRequestById([FromServices] IUpdateServiceRequestUseCase useCase, [FromRoute] Guid id, [FromBody] RequestUpdateServiceRequest request)
        {
            var response = await useCase.Execute(id, request);
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteServiceRequestById([FromServices] IDeleteServiceRequestUseCase useCase, [FromRoute] Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
            var canViewAll = User.IsInRole(UserRoles.Técnico.ToString());
            await useCase.Execute(id, userId, canViewAll);
            return NoContent();
        }   
    }
}
