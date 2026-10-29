using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BeneficiosController : ControllerBase
{
    private readonly IBeneficioService _beneficioService;

    public BeneficiosController(IBeneficioService beneficioService)
    {
        _beneficioService = beneficioService;
    }
}
