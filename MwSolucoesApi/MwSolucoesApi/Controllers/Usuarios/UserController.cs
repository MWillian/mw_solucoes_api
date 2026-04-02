using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ResponseRegisterUser), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser([FromBody] RequestRegisterUser request, [FromServices] IRegisterUserUseCase useCase)
        {
            var createdUser = await useCase.Execute(request);
            return Created(string.Empty, createdUser);
        }
        //[HttpGet]
        //[ProducesResponseType(typeof(ResponseError), StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(typeof(ResponseRegisterUser), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetUser([FromServices] I)
        //{
        //    return Ok();
        //}
    }
}
