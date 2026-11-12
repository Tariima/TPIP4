using Application.Interfaces;

namespace Infrastructure.Services;

// En Infrastructure porque depende de BCrypt (paquete externo)
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string contrasena)
    {
        return BCrypt.Net.BCrypt.HashPassword(contrasena);
    }

    public bool Verify(string contrasena, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(contrasena, hash);
    }
}
