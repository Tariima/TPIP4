using Application.Interfaces;
using Application.Models;
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

    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}