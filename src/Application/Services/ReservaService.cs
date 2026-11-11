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

    public ReservaService(IReservaRepository reservaRepository, IHorarioRepository horarioRepository, IGimnasioRepository gimnasioRepository)
    {
        _reservaRepository = reservaRepository;
        _horarioRepository = horarioRepository;
        _gimnasioRepository = gimnasioRepository;
    }

    // Horarios del gimnasio para el día de la semana de esa fecha que todavía tienen lugar
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
            // Si la fecha es hoy, los turnos que ya empezaron no se pueden reservar
            if (ArmarFechaTurno(fecha, horario) <= DateTime.Now)
            {
                continue;
            }

            int ocupados = await _reservaRepository.CountConfirmadasAsync(horario.Id, fecha);
            int cupoDisponible = horario.CupoMaximo - ocupados;

            // Los horarios que no tienen cupo no aparecen
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
        // La reserva queda confirmada directamente, no hace falta que nadie la apruebe
        nuevaReserva.Estado = EstadoReserva.Confirmada;
        nuevaReserva.UsuarioComunId = idUsuario;
        nuevaReserva.HorarioId = horario.Id;
        nuevaReserva.Horario = horario;

        var reservaCreada = await _reservaRepository.CreateAsync(nuevaReserva);
        return ReservaDTO.Create(reservaCreada);
    }

    // La reserva no se borra: queda como cancelada y el lugar vuelve a estar disponible
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
    }

    // Devuelve la reserva solo si existe y es del usuario.
    // Si es de otro usuario también da 404, así no se sabe que existe
    private async Task<Reserva> GetReservaDelUsuario(int idUsuario, int idReserva)
    {
        var reserva = await _reservaRepository.GetByIdConHorarioAsync(idReserva);

        if (reserva is null || reserva.UsuarioComunId != idUsuario)
        {
            throw new NotFoundException($"No se encontró la reserva con id {idReserva}");
        }

        return reserva;
    }

    // Nombre del día de la semana de una fecha, escrito igual que en los horarios.
    // DayOfWeek arranca en domingo (0), por eso se corre para que el lunes quede en 0 y el domingo en 6
    private string DiaDeLaSemana(DateTime fecha)
    {
        int indice = ((int)fecha.DayOfWeek + 6) % 7;
        return HorarioService.DiasSemana[indice];
    }

    // Fecha y hora en que empieza el turno: el día elegido con la hora de inicio del horario
    private DateTime ArmarFechaTurno(DateTime fecha, Horario horario)
    {
        return fecha.Date.Add(horario.HoraInicio.TimeOfDay);
    }
}
