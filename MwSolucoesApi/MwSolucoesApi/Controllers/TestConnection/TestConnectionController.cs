using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MwSolucoes.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

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
        [AllowAnonymous]
        [HttpGet("check")]
        public IActionResult CheckConnection()
        {
            try
            {
                // Recupera a Connection String que a API realmente resolveu na injeção de dependência
                var connectionString = _dbContext.Database.GetDbConnection().ConnectionString;

                // Mascara a senha para exibir com segurança no JSON de retorno
                var maskedConnectionString = System.Text.RegularExpressions.Regex.Replace(
                    connectionString,
                    @"Password=[^;]+",
                    "Password=******"
                );

                bool canConnect = _dbContext.Database.CanConnect();

                return Ok(new
                {
                    status = canConnect ? "Sucesso" : "Erro",
                    connectionStringUsada = maskedConnectionString,
                    message = canConnect
                        ? "A API conseguiu se conectar ao PostgreSQL com sucesso!"
                        : "Não foi possível conectar. Verifique se a Connection String usada acima é a mesma do DBeaver."
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { status = "Falha interna do servidor.", error = ex.Message });
            }
        }
    }
}