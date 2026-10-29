using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitudesCuentaGimnasioController : ControllerBase
{
    private readonly ISolicitudCuentaGimnasioService _solicitudCuentaGimnasioService;

    public SolicitudesCuentaGimnasioController(ISolicitudCuentaGimnasioService solicitudCuentaGimnasioService)
    {
        _solicitudCuentaGimnasioService = solicitudCuentaGimnasioService;
    }
}
