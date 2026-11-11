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

    // Ej: api/reservas/disponibles?gimnasioId=1&fecha=2026-11-16
    [HttpGet("disponibles")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<List<HorarioDisponibleDTO>>> GetHorariosDisponibles([FromQuery] int gimnasioId, [FromQuery] DateTime fecha)
    {
        return await _reservaService.GetHorariosDisponibles(gimnasioId, fecha);
    }

    [HttpGet]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<List<ReservaDTO>>> GetMisReservas()
    {
        return await _reservaService.GetMisReservas(GetIdUsuario());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<ReservaDTO>> GetById([FromRoute] int id)
    {
        return await _reservaService.GetById(GetIdUsuario(), id);
    }

    [HttpPost]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<ReservaDTO>> Create([FromBody] CreationReservaDTO creationReservaDto)
    {
        var reservaCreada = await _reservaService.Create(GetIdUsuario(), creationReservaDto);
        return Created($"api/reservas/{reservaCreada.Id}", reservaCreada);
    }

    [HttpPatch("{id}/cancelar")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult> Cancelar([FromRoute] int id)
    {
        await _reservaService.Cancelar(GetIdUsuario(), id);
        return NoContent();
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
