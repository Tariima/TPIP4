using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "UsuarioComun")]
public class RutinasController : ControllerBase
{
    private readonly IRutinaService _rutinaService;

    public RutinasController(IRutinaService rutinaService)
    {
        _rutinaService = rutinaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<RutinaDTO>>> GetAll()
    {
        return await _rutinaService.GetAll(GetIdUsuario());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RutinaDTO>> GetById([FromRoute] int id)
    {
        return await _rutinaService.GetById(GetIdUsuario(), id);
    }

    [HttpPost]
    public async Task<ActionResult<RutinaDTO>> Create([FromBody] CreationRutinaDTO creationRutinaDto)
    {
        var rutinaCreada = await _rutinaService.Create(GetIdUsuario(), creationRutinaDto);
        return Created($"api/rutinas/{rutinaCreada.Id}", rutinaCreada);
    }

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}