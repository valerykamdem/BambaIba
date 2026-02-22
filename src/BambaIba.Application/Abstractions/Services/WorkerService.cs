using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BambaIba.Application.Abstractions.Services;

public class WorkerService : BackgroundService
{

    private readonly ILogger<WorkerService> _logger;
    private readonly IMessageBus _bus; // C'est le bus Wolverine

    public WorkerService(ILogger<WorkerService> logger, IMessageBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker Service started. Waiting for media processing jobs...");

        try
        {
            // Cette ligne maintient le service en vie.
            // Pendant ce temps, Wolverine écoute la base de données.
            // Dès qu'un message "ProcessMediaCommand" arrive, 
            // Wolverine lance automatiquement ton ProcessMediaHandler.
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("🛑 [WORKER] Arrêt du service en cours...");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("⚠️ [WORKER] Arrêt demandé. Fin du traitement en cours...");
        await base.StopAsync(cancellationToken);
    }
}

