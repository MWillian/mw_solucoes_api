using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.UseCases.User.RegisterUser;
using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.User;

namespace MwSolucoes.Api.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {   
        [HttpPost]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseRegisterUser), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser([FromBody] RequestRegisterUser request, [FromServices] IRegisterUserUseCase useCase)
        {
            var createdUser = useCase.Execute(request);
            return Created(string.Empty, createdUser);
        }
    }
}
