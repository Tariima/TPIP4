using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IHorarioRepository _horarioRepository;
    private readonly IGimnasioRepository _gimnasioRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;

    public ReservaService(IReservaRepository reservaRepository, IHorarioRepository horarioRepository,
        IGimnasioRepository gimnasioRepository, IUsuarioRepository usuarioRepository, IEmailService emailService)
    {
        _reservaRepository = reservaRepository;
        _horarioRepository = horarioRepository;
        _gimnasioRepository = gimnasioRepository;
        _usuarioRepository = usuarioRepository;
        _emailService = emailService;
    }

    // Horarios de ese día de la semana que todavía tienen lugar
    public async Task<List<HorarioDisponibleDTO>> GetHorariosDisponibles(int gimnasioId, DateTime fecha)
    {
        var gimnasio = await _gimnasioRepository.GetByIdAsync(gimnasioId);

        if (gimnasio is null || !gimnasio.Activo)
        {
            throw new NotFoundException($"No se encontró el gimnasio con id {gimnasioId}");
        }

        if (fecha.Date < DateTime.Today)
        {
            throw new ValidationException("La fecha no puede ser anterior a hoy");
        }

        string dia = DiaDeLaSemana(fecha);
        var horarioList = await _horarioRepository.GetActivosByGimnasioAsync(gimnasioId);

        var disponibles = new List<HorarioDisponibleDTO>();

        foreach (var horario in horarioList.Where(h => h.DiaSemana == dia).OrderBy(h => h.HoraInicio))
        {
            // si es hoy, los que ya empezaron no
            if (ArmarFechaTurno(fecha, horario) <= DateTime.Now)
            {
                continue;
            }

            int ocupados = await _reservaRepository.CountConfirmadasAsync(horario.Id, fecha);
            int cupoDisponible = horario.CupoMaximo - ocupados;

            // sin cupo no se muestran
            if (cupoDisponible > 0)
            {
                disponibles.Add(HorarioDisponibleDTO.Create(horario, cupoDisponible));
            }
        }

        return disponibles;
    }

    public async Task<List<ReservaDTO>> GetMisReservas(int idUsuario)
    {
        var reservaList = await _reservaRepository.GetByUsuarioAsync(idUsuario);
        return ReservaDTO.CreateList(reservaList);
    }

    public async Task<ReservaDTO> GetById(int idUsuario, int idReserva)
    {
        var reserva = await GetReservaDelUsuario(idUsuario, idReserva);
        return ReservaDTO.Create(reserva);
    }

    public async Task<ReservaDTO> Create(int idUsuario, CreationReservaDTO creationReservaDto)
    {
        var horario = await _horarioRepository.GetByIdConGimnasioAsync(creationReservaDto.HorarioId);

        if (horario is null || !horario.Activo || !horario.Gimnasio.Activo)
        {
            throw new NotFoundException($"No se encontró el horario con id {creationReservaDto.HorarioId}");
        }

        string dia = DiaDeLaSemana(creationReservaDto.Fecha);

        if (horario.DiaSemana != dia)
        {
            throw new ValidationException($"El horario es para los días {horario.DiaSemana} y la fecha elegida es {dia}");
        }

        DateTime fechaTurno = ArmarFechaTurno(creationReservaDto.Fecha, horario);

        if (fechaTurno <= DateTime.Now)
        {
            throw new ValidationException("No se puede reservar un turno que ya pasó");
        }

        if (await _reservaRepository.TieneReservaConfirmadaAsync(idUsuario, creationReservaDto.Fecha))
        {
            throw new ConflictException("Ya hay una reserva confirmada para ese día");
        }

        int ocupados = await _reservaRepository.CountConfirmadasAsync(horario.Id, creationReservaDto.Fecha);

        if (ocupados >= horario.CupoMaximo)
        {
            throw new ConflictException("El horario no tiene cupo para esa fecha");
        }

        var nuevaReserva = new Reserva();

        nuevaReserva.FechaTurno = fechaTurno;
        nuevaReserva.FechaSolicitud = DateTime.Now;
        // queda confirmada directo, no hay aprobación
        nuevaReserva.Estado = EstadoReserva.Confirmada;
        nuevaReserva.UsuarioComunId = idUsuario;
        nuevaReserva.HorarioId = horario.Id;
        nuevaReserva.Horario = horario;

        var reservaCreada = await _reservaRepository.CreateAsync(nuevaReserva);

        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);

        if (usuario is not null)
        {
            await _emailService.EnviarAsync(
                usuario.Email,
                "Reserva confirmada",
                $"Hola {usuario.Nombre}, tu reserva en {horario.Gimnasio.Nombre} quedó confirmada para el {fechaTurno:dd/MM/yyyy} a las {fechaTurno:HH:mm}.");
        }

        return ReservaDTO.Create(reservaCreada);
    }

    // no se borra: pasa a Cancelada y libera el lugar
    public async Task Cancelar(int idUsuario, int idReserva)
    {
        var reserva = await GetReservaDelUsuario(idUsuario, idReserva);

        if (reserva.Estado == EstadoReserva.Cancelada)
        {
            throw new ConflictException("La reserva ya está cancelada");
        }

        if (reserva.FechaTurno <= DateTime.Now)
        {
            throw new ValidationException("Solo se puede cancelar antes del turno");
        }

        reserva.Estado = EstadoReserva.Cancelada;
        reserva.FechaCancelacion = DateTime.Now;

        await _reservaRepository.UpdateAsync(reserva);

        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);

        if (usuario is not null)
        {
            await _emailService.EnviarAsync(
                usuario.Email,
                "Reserva cancelada",
                $"Hola {usuario.Nombre}, cancelaste tu reserva en {reserva.Horario.Gimnasio.Nombre} del {reserva.FechaTurno:dd/MM/yyyy} a las {reserva.FechaTurno:HH:mm}.");
        }
    }

    // Reservas del gimnasio en una fecha, horarioId es opcional
    public async Task<List<ReservaGimnasioDTO>> GetReservasDelGimnasio(int idAdmin, DateTime? fecha, int? horarioId)
    {
        if (fecha is null)
        {
            throw new ValidationException("La fecha es requerida");
        }

        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);
        var reservaList = await _reservaRepository.GetByGimnasioAsync(gimnasioId, fecha.Value, fecha.Value);

        if (horarioId is not null)
        {
            reservaList = reservaList.Where(r => r.HorarioId == horarioId).ToList();
        }

        return ReservaGimnasioDTO.CreateList(reservaList);
    }

    public async Task<EstadisticasGimnasioDTO> GetEstadisticas(int idAdmin, DateTime? desde, DateTime? hasta)
    {
        if (desde is null || hasta is null)
        {
            throw new ValidationException("Las fechas desde y hasta son requeridas");
        }

        if (desde.Value.Date > hasta.Value.Date)
        {
            throw new ValidationException("La fecha desde no puede ser mayor a la fecha hasta");
        }

        // máximo un año, para no recorrer rangos enormes
        if ((hasta.Value.Date - desde.Value.Date).TotalDays > 366)
        {
            throw new ValidationException("El período no puede ser mayor a un año");
        }

        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);
        var reservaList = await _reservaRepository.GetByGimnasioAsync(gimnasioId, desde.Value, hasta.Value);
        var horarioList = await _horarioRepository.GetActivosByGimnasioAsync(gimnasioId);

        // se suman los horarios dados de baja que tuvieron reservas en el período
        foreach (var reserva in reservaList)
        {
            if (!horarioList.Any(h => h.Id == reserva.HorarioId))
            {
                horarioList.Add(reserva.Horario);
            }
        }

        var estadisticas = new EstadisticasGimnasioDTO();

        estadisticas.Desde = desde.Value.Date;
        estadisticas.Hasta = hasta.Value.Date;
        estadisticas.TotalReservas = reservaList.Count;
        estadisticas.Confirmadas = reservaList.Count(r => r.Estado == EstadoReserva.Confirmada);
        estadisticas.Canceladas = reservaList.Count(r => r.Estado == EstadoReserva.Cancelada);
        estadisticas.PorcentajeCancelacion = CalcularPorcentaje(estadisticas.Canceladas, estadisticas.TotalReservas);

        var horariosOrdenados = horarioList
            .OrderBy(h => Array.IndexOf(HorarioService.DiasSemana, h.DiaSemana))
            .ThenBy(h => h.HoraInicio);

        foreach (var horario in horariosOrdenados)
        {
            var reservasDelHorario = reservaList.Where(r => r.HorarioId == horario.Id).ToList();

            var ocupacion = new OcupacionHorarioDTO();

            ocupacion.HorarioId = horario.Id;
            ocupacion.DiaSemana = horario.DiaSemana;
            ocupacion.HoraInicio = horario.HoraInicio.ToString("HH:mm");
            ocupacion.HoraFin = horario.HoraFin.ToString("HH:mm");
            ocupacion.CupoMaximo = horario.CupoMaximo;
            ocupacion.CantidadTurnos = ContarDias(desde.Value, hasta.Value, horario.DiaSemana);
            ocupacion.ReservasConfirmadas = reservasDelHorario.Count(r => r.Estado == EstadoReserva.Confirmada);
            ocupacion.Cancelaciones = reservasDelHorario.Count(r => r.Estado == EstadoReserva.Cancelada);
            // ocupados / ofrecidos en el período
            ocupacion.PorcentajeOcupacion = CalcularPorcentaje(ocupacion.ReservasConfirmadas, ocupacion.CupoMaximo * ocupacion.CantidadTurnos);

            estadisticas.OcupacionPorHorario.Add(ocupacion);
        }

        return estadisticas;
    }

    // cuántas veces cae ese día entre las dos fechas (inclusive)
    private int ContarDias(DateTime desde, DateTime hasta, string diaSemana)
    {
        int cantidad = 0;

        for (var fecha = desde.Date; fecha <= hasta.Date; fecha = fecha.AddDays(1))
        {
            if (DiaDeLaSemana(fecha) == diaSemana)
            {
                cantidad++;
            }
        }

        return cantidad;
    }

    // con un decimal, 0 si el total es 0
    private double CalcularPorcentaje(int parte, int total)
    {
        if (total == 0)
        {
            return 0;
        }

        return Math.Round((double)parte * 100 / total, 1);
    }

    // sale de la cuenta del admin, no del request
    private async Task<int> GetGimnasioDelAdmin(int idAdmin)
    {
        var admin = await _usuarioRepository.GetByIdAsync(idAdmin) as AdminGimnasio;

        if (admin is null)
        {
            throw new NotFoundException("No se encontró la cuenta del admin");
        }

        return admin.GimnasioId;
    }

    // 404 si no existe o es de otro usuario
    private async Task<Reserva> GetReservaDelUsuario(int idUsuario, int idReserva)
    {
        var reserva = await _reservaRepository.GetByIdConHorarioAsync(idReserva);

        if (reserva is null || reserva.UsuarioComunId != idUsuario)
        {
            throw new NotFoundException($"No se encontró la reserva con id {idReserva}");
        }

        return reserva;
    }

    // Nombre del día como se guarda en los horarios. DayOfWeek arranca en domingo, de ahí el +6 % 7
    private string DiaDeLaSemana(DateTime fecha)
    {
        int indice = ((int)fecha.DayOfWeek + 6) % 7;
        return HorarioService.DiasSemana[indice];
    }

    // fecha elegida + hora de inicio del horario
    private DateTime ArmarFechaTurno(DateTime fecha, Horario horario)
    {
        return fecha.Date.Add(horario.HoraInicio.TimeOfDay);
    }
}
