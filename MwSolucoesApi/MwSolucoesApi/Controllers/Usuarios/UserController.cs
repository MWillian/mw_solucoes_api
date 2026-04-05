using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.UseCases.User.GetUser;
using MwSolucoes.Application.UseCases.User.GetUsers;
using MwSolucoes.Application.UseCases.User.RegisterUser;
using MwSolucoes.Application.UseCases.User.UpdateUser;
using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.User;
using System.Security.Claims;

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
            var createdUser = await useCase.Execute(request);
            return Created(string.Empty, createdUser);
        }
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseGetUser), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseGetUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMe([FromServices] IGetUserByIdUseCase useCase)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var user = await useCase.Execute(userId);
            return Ok(user);
        }

        [Authorize] //implementar verificação de admin na consulta via guid
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseGetUser), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseGetUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser([FromRoute] Guid Id, [FromServices] IGetUserByIdUseCase useCase)
        {
            var user = await useCase.Execute(Id);
            return Ok(user);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(PagedResult<ResponseGetUser>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromServices] IGetUsersUseCase useCase, [FromQuery] UserFilters filters)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out _)) return Unauthorized();

            var users = await useCase.Execute(filters);
            return Ok(users);
        }

        [Authorize]
        [HttpPut("me")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseRegisterUser), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseRegisterUser), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update([FromServices] IUpdateUserUseCase useCase)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
            return NoContent();
        }

    }
}
