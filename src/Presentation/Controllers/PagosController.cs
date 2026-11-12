using Application.Interfaces;
using Application.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

// Sin [Authorize], lo llama Mercado Pago
[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    private readonly IPagoService _pagoService;

    public PagosController(IPagoService pagoService)
    {
        _pagoService = pagoService;
    }

    /// <summary>
    /// Webhook de Mercado Pago. Lo llama Mercado Pago cuando cambia una orden, no se usa desde el front
    /// </summary>
    // hay que responder 200 rápido, si devolvemos error MP reintenta el aviso
    [HttpPost("webhook")]
    public async Task<ActionResult> Webhook([FromBody] NotificacionMercadoPagoDTO notificacion)
    {
        await _pagoService.ProcesarNotificacion(notificacion);
        return Ok();
    }
}
