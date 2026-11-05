using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}
