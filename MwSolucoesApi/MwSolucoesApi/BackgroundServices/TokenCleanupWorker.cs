using MwSolucoes.Domain.Repositories;

namespace MwSolucoes.Api.BackgroundServices
{
    public class TokenCleanupWorker : BackgroundService
    {
        private readonly ILogger<TokenCleanupWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        public TokenCleanupWorker(ILogger<TokenCleanupWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker de Limpeza de Tokens iniciado.");

            using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
            do
            {
                try
                {
                    await CleanupTokensAsync(stoppingToken);
                }
                catch (SystemException ex)
                {
                    _logger.LogError(ex, "Erro fatal durante a limpeza de Refresh Tokens.");
                }

            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task CleanupTokensAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando rotina de limpeza de Refresh Tokens.");

            using var scope = _scopeFactory.CreateScope();

            var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

            var dataLimite = DateTime.UtcNow.AddDays(-30);

            var registrosDeletados = await refreshTokenRepository.DeleteExpiredTokensAsync(dataLimite);

            _logger.LogInformation("Rotina de expurgo finalizada. {Quantidade} tokens antigos removidos.", registrosDeletados);
        }
    }
}