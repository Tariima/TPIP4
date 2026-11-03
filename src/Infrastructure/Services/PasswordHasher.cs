using Application.Interfaces;

namespace Infrastructure.Services;

// BCrypt es un paquete de terceros, por eso la implementación va en Infraestructura
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
