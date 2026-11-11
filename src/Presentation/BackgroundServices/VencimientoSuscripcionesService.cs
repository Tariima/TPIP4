using Application.Interfaces;

namespace Presentation.BackgroundServices;

// Proceso que corre solo mientras la API está prendida: al arrancar y después una vez por día.
// A un controller lo dispara un request; a este lo dispara el reloj
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
        // "Despertador" que suena cada 24 horas
        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

        // do-while: la primera vez corre apenas arranca la API, después espera al timer.
        // WaitForNextTickAsync devuelve false cuando se apaga la API y ahí termina el ciclo
        do
        {
            try
            {
                // Este proceso vive toda la vida de la API, pero el service y el DbContext
                // se crean uno por request. Por eso cada corrida arma su propio scope,
                // como si fuera un request, y al terminar se descarta
                using var scope = _scopeFactory.CreateScope();
                var suscripcionService = scope.ServiceProvider.GetRequiredService<ISuscripcionService>();

                await suscripcionService.ProcesarVencimientos();

                _logger.LogInformation("Vencimiento de suscripciones procesado");
            }
            catch (Exception ex)
            {
                // Si una corrida falla (ej: la base no responde) se registra y se sigue.
                // Sin este catch la excepción apagaría el proceso, o incluso la API
                _logger.LogError(ex, "Error al procesar el vencimiento de suscripciones");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
