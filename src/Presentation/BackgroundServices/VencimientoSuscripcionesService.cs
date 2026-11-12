using Application.Interfaces;

namespace Presentation.BackgroundServices;

// Corre al arrancar la API y después cada 24 hs (solo mientras la API está levantada)
public class VencimientoSuscripcionesService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<VencimientoSuscripcionesService> _logger;

    public VencimientoSuscripcionesService(IServiceScopeFactory scopeFactory, ILogger<VencimientoSuscripcionesService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

        // do-while para que la primera corrida sea al arrancar.
        // WaitForNextTickAsync da false cuando se apaga la API
        do
        {
            try
            {
                // Esto es singleton y el service/DbContext son scoped,
                // así que cada corrida crea su propio scope
                using var scope = _scopeFactory.CreateScope();
                var suscripcionService = scope.ServiceProvider.GetRequiredService<ISuscripcionService>();

                await suscripcionService.ProcesarVencimientos();

                _logger.LogInformation("Vencimiento de suscripciones procesado");
            }
            catch (Exception ex)
            {
                // Si falla se loguea y sigue. Sin el catch la excepción puede bajar la API
                _logger.LogError(ex, "Error al procesar el vencimiento de suscripciones");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
