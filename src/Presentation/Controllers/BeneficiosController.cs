using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
// el rol de cada endpoint se restringe abajo
[Authorize(Roles = "AdminGimnasio,UsuarioComun")]
public class BeneficiosController : ControllerBase
{
    private readonly IBeneficioService _beneficioService;

    public BeneficiosController(IBeneficioService beneficioService)
    {
        _beneficioService = beneficioService;
    }

    /// <summary>
    /// Beneficios del gimnasio del admin logueado, vigentes o no
    /// </summary>
    [HttpGet("gimnasio")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<List<BeneficioDTO>>> GetAllDelGimnasio()
    {
        return await _beneficioService.GetAllDelGimnasio(GetIdUsuario());
    }

    /// <summary>
    /// Gimnasios que hoy tienen beneficios vigentes. Solo para usuarios Pro
    /// </summary>
    [HttpGet("vigentes")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<List<GimnasioConBeneficiosDTO>>> GetVigentes()
    {
        return await _beneficioService.GetVigentes(GetIdUsuario());
    }

    /// <summary>
    /// Un beneficio del gimnasio del admin
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<BeneficioDTO>> GetById([FromRoute] int id)
    {
        return await _beneficioService.GetById(GetIdUsuario(), id);
    }

    /// <summary>
    /// Carga un beneficio en el gimnasio del admin
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult<BeneficioDTO>> Create([FromBody] CreationBeneficioDTO creationBeneficioDto)
    {
        var beneficioCreado = await _beneficioService.Create(GetIdUsuario(), creationBeneficioDto);
        return Created($"api/beneficios/{beneficioCreado.Id}", beneficioCreado);
    }

    /// <summary>
    /// Modifica un beneficio del gimnasio del admin
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "AdminGimnasio")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] CreationBeneficioDTO creationBeneficioDto)
    {
        await _beneficioService.Update(GetIdUsuario(), id, creationBeneficioDto);
        return NoContent();
    }

    /// <summary>
    /// Da de baja un beneficio (baja lógica)
    /// </summary>
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
