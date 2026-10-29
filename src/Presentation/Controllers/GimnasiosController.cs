using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GimnasiosController : ControllerBase
{
    private readonly IGimnasioService _gimnasioService;

    public GimnasiosController(IGimnasioService gimnasioService)
    {
        _gimnasioService = gimnasioService;
    }
}
