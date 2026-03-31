using Microsoft.AspNetCore.Mvc;
using MwSolucoes.Infrastructure.Data;

namespace MwSolucoes.Api.Controllers.TestConnection
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestConnectionController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public TestConnectionController(AppDbContext context)
        {
            _dbContext = context;
        }
        [HttpGet("check")]
        public IActionResult CheckConnection()
        {   
            try
            {
                bool canConnect = _dbContext.Database.CanConnect();

                if (canConnect)
                {
                    return Ok(new { status = "Sucesso", message = "A API conseguiu se conectar ao PostgreSQL com sucesso!" });
                }

                return BadRequest(new { status = "Erro", message = "A configuração está correta, mas o banco de dados não está acessível." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { status = "Falha interna do servidor.", error = ex.Message });
            }
        }
    }
}
