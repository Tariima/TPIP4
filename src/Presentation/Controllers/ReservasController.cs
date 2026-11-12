using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
// Reservar y cancelar es de UsuarioComun, ver las reservas del gimnasio es de AdminGimnasio
[Authorize(Roles = "UsuarioComun,AdminGimnasio")]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;

    public ReservasController(IReservaService reservaService)
    {
        _reservaService = reservaService;
    }

    /// <summary>
    /// Horarios de un gimnasio para una fecha, con el cupo que queda
    /// </summary>
    // Ej: api/reservas/disponibles?gimnasioId=1&fecha=2026-11-16
    [HttpGet("disponibles")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<List<HorarioDisponibleDTO>>> GetHorariosDisponibles([FromQuery] int gimnasioId, [FromQuery] DateTime fecha)
    {
        return await _reservaService.GetHorariosDisponibles(gimnasioId, fecha);
    }

    /// <summary>
    /// Reservas del usuario logueado
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<List<ReservaDTO>>> GetMisReservas()
    {
        return await _reservaService.GetMisReservas(GetIdUsuario());
    }

    /// <summary>
    /// Una reserva del usuario
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<ReservaDTO>> GetById([FromRoute] int id)
    {
        return await _reservaService.GetById(GetIdUsuario(), id);
    }

    /// <summary>
    /// Reserva un turno. Queda confirmada al momento y llega un mail
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<ReservaDTO>> Create([FromBody] CreationReservaDTO creationReservaDto)
    {
        var reservaCreada = await _reservaService.Create(GetIdUsuario(), creationReservaDto);
        return Created($"api/reservas/{reservaCreada.Id}", reservaCreada);
    }

    /// <summary>
    /// Cancela una reserva. Solo se puede antes de que empiece el turno
    /// </summary>
    [HttpPatch("{id}/cancelar")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult> Cancelar([FromRoute] int id)
    {
        await _reservaService.Cancelar(GetIdUsuario(), id);
        return NoContent();
    }

    /// <summary>
    /// Reservas del gimnasio del admin en una fecha, se puede filtrar por horario
    /// </summary>
    // Ej: api/reservas/gimnasio?fecha=2026-11-12&horarioId=3 (el horario es opcional)
    [HttpGet("gimnasio")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<List<ReservaGimnasioDTO>>> GetReservasDelGimnasio([FromQuery] DateTime? fecha, [FromQuery] int? horarioId)
    {
        return await _reservaService.GetReservasDelGimnasio(GetIdUsuario(), fecha, horarioId);
    }

    /// <summary>
    /// Estadísticas de reservas del gimnasio en un período: totales, cancelaciones y ocupación por horario
    /// </summary>
    // Ej: api/reservas/estadisticas?desde=2026-11-01&hasta=2026-11-30
    [HttpGet("estadisticas")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<EstadisticasGimnasioDTO>> GetEstadisticas([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        return await _reservaService.GetEstadisticas(GetIdUsuario(), desde, hasta);
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
