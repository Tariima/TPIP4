using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RutinasController : ControllerBase
{
    private readonly IRutinaService _rutinaService;

    public RutinasController(IRutinaService rutinaService)
    {
        _rutinaService = rutinaService;
    }
}
