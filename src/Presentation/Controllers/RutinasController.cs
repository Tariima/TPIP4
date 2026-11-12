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
public class RutinasController : ControllerBase
{
    private readonly IRutinaService _rutinaService;

    public RutinasController(IRutinaService rutinaService)
    {
        _rutinaService = rutinaService;
    }

    /// <summary>
    /// Rutinas activas del usuario
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<RutinaDTO>>> GetAll()
    {
        return await _rutinaService.GetAll(GetIdUsuario());
    }

    /// <summary>
    /// Una rutina con sus ejercicios
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RutinaDTO>> GetById([FromRoute] int id)
    {
        return await _rutinaService.GetById(GetIdUsuario(), id);
    }

    /// <summary>
    /// Crea una rutina. Tiene que tener al menos un ejercicio
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RutinaDTO>> Create([FromBody] CreationRutinaDTO creationRutinaDto)
    {
        var rutinaCreada = await _rutinaService.Create(GetIdUsuario(), creationRutinaDto);
        return Created($"api/rutinas/{rutinaCreada.Id}", rutinaCreada);
    }

    /// <summary>
    /// Modifica una rutina. La lista de ejercicios reemplaza a la anterior
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateRutinaDTO updateRutinaDto)
    {
        await _rutinaService.Update(GetIdUsuario(), id, updateRutinaDto);
        return NoContent();
    }

    /// <summary>
    /// Baja lógica de una rutina
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _rutinaService.Delete(GetIdUsuario(), id);
        return NoContent();
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}