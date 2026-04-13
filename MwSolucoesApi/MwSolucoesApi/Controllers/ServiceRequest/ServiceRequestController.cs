using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.UseCases.ServiceRequest;
using MwSolucoes.Communication.Requests.ServiceRequest;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.ServiceRequest;
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
    }
}
