using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.UseCases.Auth.UpdatePassword;
using MwSolucoes.Communication.Requests.Auth;
using MwSolucoes.Communication.Responses;
using System.Security.Claims;

namespace MwSolucoes.Api.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePassword([FromBody] RequestUpdatePassword request, [FromServices] IUpdatePasswordUseCase useCase)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            await useCase.Execute(userId, request);

            return NoContent();
        }
    }
}
