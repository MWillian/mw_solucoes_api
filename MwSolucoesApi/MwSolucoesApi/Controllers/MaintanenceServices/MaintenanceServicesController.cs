using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Application.UseCases.MaintenanceService.Create;
using MwSolucoes.Application.UseCases.MaintenanceService.Deactivate;
using MwSolucoes.Application.UseCases.MaintenanceService.Delete;
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
    }
}
