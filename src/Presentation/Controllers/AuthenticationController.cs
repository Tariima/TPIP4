using Application.Interfaces;
using Application.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly ICustomAuthenticationService _authenticationService;

    public AuthenticationController(ICustomAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Login. Devuelve el token JWT (dura 1 hora) que se pega en el botón Authorize
    /// </summary>
    [HttpPost("authenticate")]
    public async Task<ActionResult<string>> Autenticar([FromBody] CredentialsDTO credentials)
    {
        string token = await _authenticationService.Autenticar(credentials);
        return Ok(token);
    }
}
