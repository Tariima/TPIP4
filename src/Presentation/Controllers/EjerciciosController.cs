using Application.Interfaces;
using Application.Models;
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

    /// <summary>
    /// Lista los ejercicios. Se puede filtrar por grupo muscular (ej: Pecho, Espalda, Piernas)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<EjercicioDTO>>> GetAll([FromQuery] string? grupoMuscular)
    {
        return await _ejercicioService.GetAll(grupoMuscular);
    }

    /// <summary>
    /// Un ejercicio por id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EjercicioDTO>> GetById([FromRoute] int id)
    {
        return await _ejercicioService.GetById(id);
    }
}
