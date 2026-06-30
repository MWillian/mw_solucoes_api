using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.UseCases.User.GetUser;
using MwSolucoes.Communication.Requests.User;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.User;
using System.Security.Claims;

namespace MwSolucoes.Api.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseRegisterUser), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] RequestRegisterUser request)
        {
            var createdUser = await _userService.RegisterUser(request);
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

        [Authorize(Policy = "AdminAccess")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseGetUser), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseGetUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser([FromRoute] Guid Id, [FromServices] IGetUserByIdUseCase useCase)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
            var user = await useCase.Execute(Id);
            return Ok(user);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(PagedResult<ResponseGetUser>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilters filters)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out _)) return Unauthorized();

            var users = await _userService.GetUserList(filters);
            return Ok(users);
        }

        [Authorize]
        [HttpPut("me")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseRegisterUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] RequestUpdateUser request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
            var updatedUser = await _userService.UpdateUser(userId, request);
            return Ok(updatedUser);
        }

        [Authorize(Policy = "AdminAccess")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var _)) return Unauthorized();
            await _userService.DeactivateUser(id);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("me")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeactivateMe()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
            await _userService.DeactivateUser(userId);
            return NoContent();
        }
    }
}
