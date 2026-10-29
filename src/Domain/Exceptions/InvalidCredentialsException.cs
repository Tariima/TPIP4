namespace Domain.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Email o contraseña incorrectos")
    {
    }
}