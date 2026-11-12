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

    /// <summary>
    /// Registro de un usuario nuevo. No pide token
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UsuarioDTO>> Create([FromBody] CreationUsuarioDTO creationUsuarioDto)
    {
        var usuarioCreado = await _usuarioService.Create(creationUsuarioDto);
        return Created($"api/usuarios/{usuarioCreado.Id}", usuarioCreado);
    }

    /// <summary>
    /// Perfil del usuario logueado
    /// </summary>
    [HttpGet("perfil")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult<UsuarioComunDTO>> GetPerfil()
    {
        return await _usuarioService.GetById(GetIdUsuario());
    }

    /// <summary>
    /// Modifica los datos del perfil. El email no se puede cambiar
    /// </summary>
    [HttpPut("perfil")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult> UpdatePerfil([FromBody] UpdateUsuarioDTO updateUsuarioDto)
    {
        await _usuarioService.Update(GetIdUsuario(), updateUsuarioDto);
        return NoContent();
    }

    /// <summary>
    /// Cambia la contraseña. Hay que mandar la actual
    /// </summary>
    [HttpPut("perfil/contrasena")]
    [Authorize(Roles = "UsuarioComun")]
    public async Task<ActionResult> UpdateContrasena([FromBody] UpdateContrasenaDTO updateContrasenaDto)
    {
        await _usuarioService.UpdateContrasena(GetIdUsuario(), updateContrasenaDto);
        return NoContent();
    }

    // el id viene en el claim "sub", ASP.NET lo mapea a NameIdentifier
    private int GetIdUsuario()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
