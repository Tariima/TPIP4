using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
// Crear, modificar y desactivar horarios se limita a AdminGimnasio en cada endpoint
[Authorize(Roles = "AdminGimnasio,UsuarioComun")]
public class HorariosController : ControllerBase
{
    private readonly IHorarioService _horarioService;

    public HorariosController(IHorarioService horarioService)
    {
        _horarioService = horarioService;
    }

    /// <summary>
    /// Horarios del gimnasio del admin
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<List<HorarioDTO>>> GetAll()
    {
        return await _horarioService.GetAll(GetIdUsuario());
    }

    /// <summary>
    /// Un horario del gimnasio del admin
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<HorarioDTO>> GetById([FromRoute] int id)
    {
        return await _horarioService.GetById(GetIdUsuario(), id);
    }

    /// <summary>
    /// Crea un horario. Las horas van como texto HH:mm (ej: 08:30)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<HorarioDTO>> Create([FromBody] CreationHorarioDTO creationHorarioDto)
    {
        var horarioCreado = await _horarioService.Create(GetIdUsuario(), creationHorarioDto);
        return Created($"api/horarios/{horarioCreado.Id}", horarioCreado);
    }

    /// <summary>
    /// Modifica un horario
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] CreationHorarioDTO creationHorarioDto)
    {
        await _horarioService.Update(GetIdUsuario(), id, creationHorarioDto);
        return NoContent();
    }

    /// <summary>
    /// Desactiva un horario
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _horarioService.Delete(GetIdUsuario(), id);
        return NoContent();
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
