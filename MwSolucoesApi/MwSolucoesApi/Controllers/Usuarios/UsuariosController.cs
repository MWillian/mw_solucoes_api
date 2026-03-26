using Microsoft.AspNetCore.Mvc;

namespace MwSolucoes.Api.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

    }
}
