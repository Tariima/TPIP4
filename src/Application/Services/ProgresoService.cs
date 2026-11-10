using Application.Interfaces;
using Application.Models;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

// Arma los datos de las gráficas a partir de los entrenamientos que ya están guardados
public class ProgresoService : IProgresoService
{
    private readonly IEntrenamientoRepository _entrenamientoRepository;
    private readonly ISuscripcionRepository _suscripcionRepository;
    private readonly IEjercicioRepository _ejercicioRepository;

    public ProgresoService(IEntrenamientoRepository entrenamientoRepository, ISuscripcionRepository suscripcionRepository, IEjercicioRepository ejercicioRepository)
    {
        _entrenamientoRepository = entrenamientoRepository;
        _suscripcionRepository = suscripcionRepository;
        _ejercicioRepository = ejercicioRepository;
    }

    public async Task<List<ProgresoEjercicioDTO>> GetProgresoEjercicio(int idUsuario, int idEjercicio, DateTime? desde, DateTime? hasta)
    {
        await VerificarUsuarioPro(idUsuario);
        ValidarFechas(desde, hasta);

        var ejercicio = await _ejercicioRepository.GetByIdAsync(idEjercicio);

        if (ejercicio is null)
        {
            throw new NotFoundException($"No se encontró el ejercicio con id {idEjercicio}");
        }

        var entrenamientos = await _entrenamientoRepository.GetByUsuarioAsync(idUsuario, desde, hasta);

        // 1. De cada entrenamiento se toman solo las series de este ejercicio, junto con el día
        // 2. Se agrupan por día (si entrenó dos veces el mismo día cuenta como un solo punto)
        // 3. De cada día se saca el peso máximo y el volumen
        return entrenamientos
            .SelectMany(e => e.RegistrosSerie
                .Where(rs => rs.EjercicioId == idEjercicio)
                .Select(rs => new { Fecha = e.Fecha.Date, rs.Peso, rs.Repeticiones }))
            .GroupBy(s => s.Fecha)
            .OrderBy(g => g.Key)
            .Select(g => new ProgresoEjercicioDTO
            {
                Fecha = g.Key,
                PesoMaximo = g.Max(s => s.Peso),
                Volumen = g.Sum(s => s.Peso * s.Repeticiones)
            })
            .ToList();
    }

    public async Task<List<ResumenSemanalDTO>> GetResumenSemanal(int idUsuario, DateTime? desde, DateTime? hasta)
    {
        await VerificarUsuarioPro(idUsuario);
        ValidarFechas(desde, hasta);

        var entrenamientos = await _entrenamientoRepository.GetByUsuarioAsync(idUsuario, desde, hasta);

        // Se agrupan los entrenamientos por el lunes de su semana y se suman los datos de cada grupo
        return entrenamientos
            .GroupBy(e => InicioDeSemana(e.Fecha))
            .OrderBy(g => g.Key)
            .Select(g => new ResumenSemanalDTO
            {
                InicioSemana = g.Key,
                CantidadEntrenamientos = g.Count(),
                MinutosTotales = g.Sum(e => e.DuracionMinutos),
                VolumenTotal = g.Sum(e => e.RegistrosSerie.Sum(rs => rs.Peso * rs.Repeticiones))
            })
            .ToList();
    }

    private async Task VerificarUsuarioPro(int idUsuario)
    {
        if (!await _suscripcionRepository.TieneSuscripcionActivaAsync(idUsuario))
        {
            throw new ForbiddenException("Las gráficas de progreso son solo para usuarios Pro");
        }
    }

    private void ValidarFechas(DateTime? desde, DateTime? hasta)
    {
        if (desde is not null && hasta is not null && desde > hasta)
        {
            throw new ValidationException("La fecha desde no puede ser posterior a la fecha hasta");
        }
    }

    // Devuelve el lunes de la semana de esa fecha. DayOfWeek arranca en domingo (0),
    // por eso se corre para que el lunes quede en 0 y el domingo en 6
    private DateTime InicioDeSemana(DateTime fecha)
    {
        int diasDesdeLunes = ((int)fecha.DayOfWeek + 6) % 7;
        return fecha.Date.AddDays(-diasDesdeLunes);
    }
}
