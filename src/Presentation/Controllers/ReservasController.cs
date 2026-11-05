using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
// Reservar y cancelar es de UsuarioComun, ver las reservas del gimnasio es de AdminGimnasio
[Authorize(Roles = "UsuarioComun,AdminGimnasio")]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;

    public ReservasController(IReservaService reservaService)
    {
        _reservaService = reservaService;
    }
}
