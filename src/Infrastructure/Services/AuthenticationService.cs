using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Application.Models.Request;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthenticationService : ICustomAuthenticationService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _config;

    public AuthenticationService(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher, IConfiguration config)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _config = config;
    }

    public async Task<string> Autenticar(CredentialsDTO credentials)
    {
        // 1. ¿Existe el usuario, está activo y la contraseña es correcta?
        var usuario = await _usuarioRepository.GetByEmailAsync(credentials.Email);

        if (usuario is null || !usuario.Activo || !_passwordHasher.Verify(credentials.Contrasena, usuario.Contrasena))
        {
            throw new InvalidCredentialsException();
        }

        // 2. Clave secreta + algoritmo de firma
        var securityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_config["Authentication:SecretForKey"]!));
        var signature = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 3. Claims que viajan en el payload. El rol es el tipo de usuario (UsuarioComun, AdminGimnasio o SuperAdmin)
        var claimsForToken = new List<Claim>
        {
            new("sub", usuario.Id.ToString()),
            new("given_name", usuario.Nombre),
            new("role", usuario.GetType().Name)
        };

        // 4. El token: issuer, audience, claims, desde, hasta y firma
        var jwtSecurityToken = new JwtSecurityToken(
            _config["Authentication:Issuer"],
            _config["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signature);

        // 5. Serializarlo a header.payload.signature
        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
