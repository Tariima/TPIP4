using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

// Solo Pro, lo valida el service
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "UsuarioComun")]
public class ProgresoController : ControllerBase
{
    private readonly IProgresoService _progresoService;

    public ProgresoController(IProgresoService progresoService)
    {
        _progresoService = progresoService;
    }

    /// <summary>
    /// Peso máximo y volumen por día de un ejercicio, para la gráfica. Solo Pro
    /// </summary>
    [HttpGet("ejercicios/{idEjercicio}")]
    public async Task<ActionResult<List<ProgresoEjercicioDTO>>> GetProgresoEjercicio([FromRoute] int idEjercicio, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        return await _progresoService.GetProgresoEjercicio(GetIdUsuario(), idEjercicio, desde, hasta);
    }

    /// <summary>
    /// Resumen por semana: entrenamientos, minutos y volumen. Solo Pro
    /// </summary>
    [HttpGet("semanal")]
    public async Task<ActionResult<List<ResumenSemanalDTO>>> GetResumenSemanal([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        return await _progresoService.GetResumenSemanal(GetIdUsuario(), desde, hasta);
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
