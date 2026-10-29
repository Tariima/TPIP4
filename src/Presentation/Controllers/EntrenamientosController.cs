using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntrenamientosController : ControllerBase
{
    private readonly IEntrenamientoService _entrenamientoService;

    public EntrenamientosController(IEntrenamientoService entrenamientoService)
    {
        _entrenamientoService = entrenamientoService;
    }
}
