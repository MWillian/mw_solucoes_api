using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.UseCases.MaintenanceService.Create;
using MwSolucoes.Application.UseCases.MaintenanceService.Deactivate;
using MwSolucoes.Application.UseCases.MaintenanceService.Delete;
using MwSolucoes.Application.UseCases.MaintenanceService.GetMaintenanceService;
using MwSolucoes.Application.UseCases.MaintenanceService.GetMaintenanceServices;
using MwSolucoes.Application.UseCases.MaintenanceService.Reactivate;
using MwSolucoes.Application.UseCases.MaintenanceService.Update;
using MwSolucoes.Communication.Requests.MaintenanceService;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.MaintenanceService;

namespace MwSolucoes.Api.Controllers.MaintanenceServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceServicesController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(PagedResult<ResponseGetMaintenanceService>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMaintenanceServices([FromServices] IGetMaintenanceServicesUseCase useCase, [FromQuery] MaintenanceServiceFilters filters)
        {
            var response = await useCase.Execute(filters);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseGetMaintenanceService), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMaintenanceServiceById([FromRoute] int id, [FromServices] IGetMaintenanceServiceByIdUseCase useCase)
        {
            var response = await useCase.Execute(id);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseCreateMaintenanceService), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateMaintenanceService([FromServices] ICreateMaintenanceServiceUseCase useCase, [FromBody] RequestCreateMaintenanceService request)
        {
            var response = await useCase.Execute(request);
            return Created(string.Empty, response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteMaintenanceService([FromRoute] int id, [FromServices] IDeleteMaintenanceServiceUseCase useCase)
        {
            await useCase.Execute(id);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ResponseUpdateMaintenanceService), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMaintenanceService([FromRoute] int id, [FromServices] IUpdateMaintenanceServiceUseCase useCase, [FromBody] RequestUpdateMaintenanceService request)
        {
            var response = await useCase.Execute(id, request);
            return Ok(response);
        }

        [HttpPatch("{id:int}/deactivate")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeactivateMaintenanceService([FromRoute] int id, [FromServices] IDeactivateMaintenanceServiceUseCase useCase)
        {
            await useCase.Execute(id);
            return NoContent();
        }

        [HttpPatch("{id:int}/reactivate")]
        [Authorize(Roles = "Técnico")]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ReactivateMaintenanceService([FromRoute] int id, [FromServices] IReactivateMaintenanceServiceUseCase useCase)
        {
            await useCase.Execute(id);
            return NoContent();
        }
    }
}
