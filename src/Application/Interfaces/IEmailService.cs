namespace Application.Interfaces;

// Envío de mails. Si el envío falla la implementación no tiene que tirar excepción,
// porque un mail que no salió no puede hacer fallar la operación que lo disparó
public interface IEmailService
{
    Task EnviarAsync(string destinatario, string asunto, string cuerpo);
}
