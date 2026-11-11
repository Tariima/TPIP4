using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
// Cargar, editar y dar de baja beneficios se limita a AdminGimnasio en cada endpoint
[Authorize(Roles = "AdminGimnasio,UsuarioComun")]
public class BeneficiosController : ControllerBase
{
    private readonly IBeneficioService _beneficioService;

    public BeneficiosController(IBeneficioService beneficioService)
    {
        _beneficioService = beneficioService;
    }

    // Los beneficios del gimnasio del admin (vigentes o no)
    [HttpGet("gimnasio")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<List<BeneficioDTO>>> GetAllDelGimnasio()
    {
        return await _beneficioService.GetAllDelGimnasio(GetIdUsuario());
    }

    // Para usuarios Pro: gimnasios con beneficios vigentes hoy
    [HttpGet("vigentes")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<List<GimnasioConBeneficiosDTO>>> GetVigentes()
    {
        return await _beneficioService.GetVigentes(GetIdUsuario());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<BeneficioDTO>> GetById([FromRoute] int id)
    {
        return await _beneficioService.GetById(GetIdUsuario(), id);
    }

    [HttpPost]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<BeneficioDTO>> Create([FromBody] CreationBeneficioDTO creationBeneficioDto)
    {
        var beneficioCreado = await _beneficioService.Create(GetIdUsuario(), creationBeneficioDto);
        return Created($"api/beneficios/{beneficioCreado.Id}", beneficioCreado);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] CreationBeneficioDTO creationBeneficioDto)
    {
        await _beneficioService.Update(GetIdUsuario(), id, creationBeneficioDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _beneficioService.Delete(GetIdUsuario(), id);
        return NoContent();
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
