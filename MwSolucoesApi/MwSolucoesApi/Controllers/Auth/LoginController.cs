using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.UseCases.Auth;
using MwSolucoes.Application.UseCases.Auth.UpdatePassword;
using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.User;
using System.Security.Claims;

namespace MwSolucoes.Api.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseLogin), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] RequestLogin request, [FromServices] ILoginUseCase loginUseCase)
        {
            var response = await loginUseCase.Execute(request);
            return Ok(response);
        }

        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseLogin), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePassword([FromBody] RequestUpdatePassword request, [FromServices] IUpdatePasswordUseCase useCase)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            await useCase.Execute(userId, request);

            return Ok();
        }
    }
}
