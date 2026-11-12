using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
// alta, modificación y baja: solo SuperAdmin
[Authorize(Roles = "SuperAdmin,UsuarioComun")]
public class GimnasiosController : ControllerBase
{
    private readonly IGimnasioService _gimnasioService;

    public GimnasiosController(IGimnasioService gimnasioService)
    {
        _gimnasioService = gimnasioService;
    }

    /// <summary>
    /// Gimnasios activos. No pide token
    /// </summary>
    // público: el responsable lo necesita para elegir el gimnasio al pedir la cuenta
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<GimnasioDTO>>> GetActivos()
    {
        return await _gimnasioService.GetActivos();
    }

    /// <summary>
    /// Todos los gimnasios, también los dados de baja
    /// </summary>
    [HttpGet("todos")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<List<GimnasioDTO>>> GetAll()
    {
        return await _gimnasioService.GetAll();
    }

    /// <summary>
    /// Un gimnasio por id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GimnasioDTO>> GetById([FromRoute] int id)
    {
        return await _gimnasioService.GetById(id);
    }

    /// <summary>
    /// Alta de un gimnasio
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<GimnasioDTO>> Create([FromBody] CreationGimnasioDTO creationGimnasioDto)
    {
        var gimnasioCreado = await _gimnasioService.Create(GetIdUsuario(), creationGimnasioDto);
        return Created($"api/gimnasios/{gimnasioCreado.Id}", gimnasioCreado);
    }

    /// <summary>
    /// Modifica los datos de un gimnasio
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] CreationGimnasioDTO creationGimnasioDto)
    {
        await _gimnasioService.Update(id, creationGimnasioDto);
        return NoContent();
    }

    /// <summary>
    /// Baja lógica de un gimnasio. También desactiva sus horarios y beneficios
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _gimnasioService.Delete(id);
        return NoContent();
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
