using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MwSolucoes.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        protected Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Sid);
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;

            }
            throw new UnauthorizedAccessException("Claim do ID do usuário está ausente ou inválida.");
        }
    }
}
