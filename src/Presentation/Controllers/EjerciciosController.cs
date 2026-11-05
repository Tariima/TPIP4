using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "UsuarioComun")]
public class EjerciciosController : ControllerBase
{
    private readonly IEjercicioService _ejercicioService;

    public EjerciciosController(IEjercicioService ejercicioService)
    {
        _ejercicioService = ejercicioService;
    }
}
