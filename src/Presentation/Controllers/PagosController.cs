using Application.Interfaces;
using Application.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

// Sin [Authorize]: a este controller lo llama Mercado Pago, no un usuario logueado
[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    private readonly IPagoService _pagoService;

    public PagosController(IPagoService pagoService)
    {
        _pagoService = pagoService;
    }

    // Webhook: Mercado Pago llama acá cuando cambia el estado de una orden.
    // Hay que responder 200 rápido; si responde error, Mercado Pago vuelve a mandar el aviso más tarde
    [HttpPost("webhook")]
    public async Task<ActionResult> Webhook([FromBody] NotificacionMercadoPagoDTO notificacion)
    {
        await _pagoService.ProcesarNotificacion(notificacion);
        return Ok();
    }
}
