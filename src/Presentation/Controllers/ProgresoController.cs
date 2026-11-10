using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

// Datos para las gráficas de progreso. Solo para usuarios Pro (lo controla el service)
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

    [HttpGet("ejercicios/{idEjercicio}")]
    public async Task<ActionResult<List<ProgresoEjercicioDTO>>> GetProgresoEjercicio([FromRoute] int idEjercicio, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        return await _progresoService.GetProgresoEjercicio(GetIdUsuario(), idEjercicio, desde, hasta);
    }

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
