using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "UsuarioComun")]
public class EntrenamientosController : ControllerBase
{
    private readonly IEntrenamientoService _entrenamientoService;

    public EntrenamientosController(IEntrenamientoService entrenamientoService)
    {
        _entrenamientoService = entrenamientoService;
    }

    /// <summary>
    /// Historial de entrenamientos del usuario, con filtro opcional por fechas
    /// </summary>
    // Historial: las fechas van en la URL y son opcionales, ej: api/entrenamientos?desde=2026-11-01&hasta=2026-11-30
    [HttpGet]
    public async Task<ActionResult<List<EntrenamientoDTO>>> GetAll([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        return await _entrenamientoService.GetAll(GetIdUsuario(), desde, hasta);
    }

    /// <summary>
    /// Un entrenamiento con sus series
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EntrenamientoDTO>> GetById([FromRoute] int id)
    {
        return await _entrenamientoService.GetById(GetIdUsuario(), id);
    }

    /// <summary>
    /// Registra un entrenamiento con las series que se hicieron
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EntrenamientoDTO>> Create([FromBody] CreationEntrenamientoDTO creationEntrenamientoDto)
    {
        var entrenamientoCreado = await _entrenamientoService.Create(GetIdUsuario(), creationEntrenamientoDto);
        return Created($"api/entrenamientos/{entrenamientoCreado.Id}", entrenamientoCreado);
    }

    /// <summary>
    /// Modifica un entrenamiento. La lista de series reemplaza a la anterior
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateEntrenamientoDTO updateEntrenamientoDto)
    {
        await _entrenamientoService.Update(GetIdUsuario(), id, updateEntrenamientoDto);
        return NoContent();
    }

    /// <summary>
    /// Borra un entrenamiento
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _entrenamientoService.Delete(GetIdUsuario(), id);
        return NoContent();
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
