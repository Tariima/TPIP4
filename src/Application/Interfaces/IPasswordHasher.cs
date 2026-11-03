namespace Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string contrasena);
    bool Verify(string contrasena, string hash);
}
