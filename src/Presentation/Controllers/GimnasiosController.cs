using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
// El alta, modificación y baja se limitan a SuperAdmin en cada endpoint
[Authorize(Roles = "SuperAdmin,UsuarioComun")]
public class GimnasiosController : ControllerBase
{
    private readonly IGimnasioService _gimnasioService;

    public GimnasiosController(IGimnasioService gimnasioService)
    {
        _gimnasioService = gimnasioService;
    }
}
