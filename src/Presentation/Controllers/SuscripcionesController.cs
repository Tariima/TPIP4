using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "UsuarioComun")]
public class SuscripcionesController : ControllerBase
{
    private readonly ISuscripcionService _suscripcionService;

    public SuscripcionesController(ISuscripcionService suscripcionService)
    {
        _suscripcionService = suscripcionService;
    }

    /// <summary>
    /// La suscripción del usuario con todos sus pagos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<SuscripcionDTO>> GetMiSuscripcion()
    {
        return await _suscripcionService.GetMiSuscripcion(GetIdUsuario());
    }

    /// <summary>
    /// Contratar o renovar Pro: devuelve el link de Mercado Pago para pagar un mes
    /// </summary>
    [HttpPost("pagar")]
    public async Task<ActionResult<LinkPagoDTO>> Pagar()
    {
        return await _suscripcionService.Pagar(GetIdUsuario());
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
