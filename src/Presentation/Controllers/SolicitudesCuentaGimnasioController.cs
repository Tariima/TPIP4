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

    // Es público porque el responsable del gimnasio todavía no tiene cuenta
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<SolicitudCuentaGimnasioDTO>> Create([FromBody] CreationSolicitudCuentaGimnasioDTO creationSolicitudDto)
    {
        var solicitudCreada = await _solicitudCuentaGimnasioService.Create(creationSolicitudDto);
        return Created($"api/solicitudescuentagimnasio/{solicitudCreada.Id}", solicitudCreada);
    }

    [HttpGet]
    public async Task<ActionResult<List<SolicitudCuentaGimnasioDTO>>> GetAll([FromQuery] string? estado)
    {
        return await _solicitudCuentaGimnasioService.GetAll(estado);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SolicitudCuentaGimnasioDTO>> GetById([FromRoute] int id)
    {
        return await _solicitudCuentaGimnasioService.GetById(id);
    }

    [HttpPatch("{id}/aprobar")]
    public async Task<ActionResult> Aprobar([FromRoute] int id)
    {
        await _solicitudCuentaGimnasioService.Aprobar(GetIdUsuario(), id);
        return NoContent();
    }

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
