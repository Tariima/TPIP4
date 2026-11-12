using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class SolicitudesCuentaGimnasioController : ControllerBase
{
    private readonly ISolicitudCuentaGimnasioService _solicitudCuentaGimnasioService;

    public SolicitudesCuentaGimnasioController(ISolicitudCuentaGimnasioService solicitudCuentaGimnasioService)
    {
        _solicitudCuentaGimnasioService = solicitudCuentaGimnasioService;
    }

    /// <summary>
    /// El responsable de un gimnasio pide su cuenta de admin. Queda inactiva hasta que la aprueben
    /// </summary>
    // público, el responsable todavía no tiene cuenta
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<SolicitudCuentaGimnasioDTO>> Create([FromBody] CreationSolicitudCuentaGimnasioDTO creationSolicitudDto)
    {
        var solicitudCreada = await _solicitudCuentaGimnasioService.Create(creationSolicitudDto);
        return Created($"api/solicitudescuentagimnasio/{solicitudCreada.Id}", solicitudCreada);
    }

    /// <summary>
    /// Lista las solicitudes. Se puede filtrar por estado (Pendiente, Aprobada o Rechazada)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<SolicitudCuentaGimnasioDTO>>> GetAll([FromQuery] string? estado)
    {
        return await _solicitudCuentaGimnasioService.GetAll(estado);
    }

    /// <summary>
    /// Una solicitud por id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SolicitudCuentaGimnasioDTO>> GetById([FromRoute] int id)
    {
        return await _solicitudCuentaGimnasioService.GetById(id);
    }

    /// <summary>
    /// Aprueba la solicitud, activa la cuenta del admin y le manda un mail
    /// </summary>
    [HttpPatch("{id}/aprobar")]
    public async Task<ActionResult> Aprobar([FromRoute] int id)
    {
        await _solicitudCuentaGimnasioService.Aprobar(GetIdUsuario(), id);
        return NoContent();
    }

    /// <summary>
    /// Rechaza la solicitud
    /// </summary>
    [HttpPatch("{id}/rechazar")]
    public async Task<ActionResult> Rechazar([FromRoute] int id)
    {
        await _solicitudCuentaGimnasioService.Rechazar(GetIdUsuario(), id);
        return NoContent();
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
