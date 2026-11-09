using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [HttpGet("perfil")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<UsuarioComunDTO>> GetPerfil()
    {
        return await _usuarioService.GetById(GetIdUsuario());
    }

    [HttpPut("perfil")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult> UpdatePerfil([FromBody] UpdateUsuarioDTO updateUsuarioDto)
    {
        await _usuarioService.Update(GetIdUsuario(), updateUsuarioDto);
        return NoContent();
    }

    [HttpPut("perfil/contrasena")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult> UpdateContrasena([FromBody] UpdateContrasenaDTO updateContrasenaDto)
    {
        await _usuarioService.UpdateContrasena(GetIdUsuario(), updateContrasenaDto);
        return NoContent();
    }

    // El id del usuario logueado viene en el claim "sub" del token.
    // Al validar el token, ASP.NET lo guarda en User con el tipo ClaimTypes.NameIdentifier
    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
