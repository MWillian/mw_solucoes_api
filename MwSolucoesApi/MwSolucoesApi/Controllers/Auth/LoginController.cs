using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.UseCases.Auth;
using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.User;

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
    }
}
