using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDTO>> Create([FromBody] CreationUsuarioDTO creationUsuarioDto)
    {
        var usuarioCreado = await _usuarioService.Create(creationUsuarioDto);
        return Created($"api/usuarios/{usuarioCreado.Id}", usuarioCreado);
    }
}
