using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
// El alta, modificación y baja se limitan a SuperAdmin en cada endpoint
[Authorize(Roles = "SuperAdmin,UsuarioComun")]
public class GimnasiosController : ControllerBase
{
    private readonly IGimnasioService _gimnasioService;

    public GimnasiosController(IGimnasioService gimnasioService)
    {
        _gimnasioService = gimnasioService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GimnasioDTO>>> GetActivos()
    {
        return await _gimnasioService.GetActivos();
    }

    [HttpGet("todos")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<List<GimnasioDTO>>> GetAll()
    {
        return await _gimnasioService.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GimnasioDTO>> GetById([FromRoute] int id)
    {
        return await _gimnasioService.GetById(id);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<GimnasioDTO>> Create([FromBody] CreationGimnasioDTO creationGimnasioDto)
    {
        var gimnasioCreado = await _gimnasioService.Create(GetIdUsuario(), creationGimnasioDto);
        return Created($"api/gimnasios/{gimnasioCreado.Id}", gimnasioCreado);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] CreationGimnasioDTO creationGimnasioDto)
    {
        await _gimnasioService.Update(id, creationGimnasioDto);
        return NoContent();
    }

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
