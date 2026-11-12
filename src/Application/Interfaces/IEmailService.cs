namespace Application.Interfaces;

// Las implementaciones no tiran excepción: si un mail falla no puede romper lo que lo mandó
public interface IEmailService
{
    Task EnviarAsync(string destinatario, string asunto, string cuerpo);
}
