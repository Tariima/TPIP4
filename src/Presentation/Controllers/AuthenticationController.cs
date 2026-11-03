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

    [HttpPost("authenticate")]
    public async Task<ActionResult<string>> Autenticar([FromBody] CredentialsDTO credentials)
    {
        string token = await _authenticationService.Autenticar(credentials);
        return Ok(token);
    }
}
