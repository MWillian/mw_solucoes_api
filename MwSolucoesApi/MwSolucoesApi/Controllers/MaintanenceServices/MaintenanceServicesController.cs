using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.Interfaces;
using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.MaintenanceService;

namespace MwSolucoes.Api.Controllers.MaintanenceServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceServicesController : MainController
    {
        private readonly IMaintenanceService _maintenanceService;
        public MaintenanceServicesController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(PagedResult<ResponseGetMaintenanceService>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMaintenanceServices([FromQuery] MaintenanceServiceFilters filters)
        {
            var userId = GetUserId();

            var response = await _maintenanceService.GetMaintenanceServices(filters);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseGetMaintenanceService), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMaintenanceServiceById([FromRoute] int id)
        {
            var userId = GetUserId();

            var response = await _maintenanceService.GetMaintenanceServiceById(id);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseCreateMaintenanceService), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateMaintenanceService([FromBody] RequestCreateMaintenanceService request)
        {
            var userId = GetUserId();

            var response = await _maintenanceService.CreateMaintenanceService(request);
            return Created(string.Empty, response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteMaintenanceService([FromRoute] int id)
        {
            var userId = GetUserId();

            await _maintenanceService.DeleteMaintenanceService(id);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ResponseUpdateMaintenanceService), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMaintenanceService([FromRoute] int id, [FromBody] RequestUpdateMaintenanceService request)
        {
            var userId = GetUserId();

            var response = await _maintenanceService.UpdateMaintenanceService(id, request);
            return Ok(response);
        }

        [HttpPatch("{id:int}/deactivate")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeactivateMaintenanceService([FromRoute] int id)
        {
            var userId = GetUserId();

            await _maintenanceService.DeactivateMaintenanceService(id);
            return NoContent();
        }

        [HttpPatch("{id:int}/reactivate")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ReactivateMaintenanceService([FromRoute] int id)
        {
            var userId = GetUserId();

            await _maintenanceService.Reactivate(id);
            return NoContent();
        }
    }
}
