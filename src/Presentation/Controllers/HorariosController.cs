using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
// Crear, modificar y desactivar horarios se limita a AdminGimnasio en cada endpoint
[Authorize(Roles = "AdminGimnasio,UsuarioComun")]
public class HorariosController : ControllerBase
{
    private readonly IHorarioService _horarioService;

    public HorariosController(IHorarioService horarioService)
    {
        _horarioService = horarioService;
    }
}
