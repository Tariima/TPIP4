using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

// Implementación provisoria: en vez de mandar el mail lo escribe en el log de la API.
// Cuando esté el servicio de mails real (#132) se reemplaza esta clase en Program.cs
// y el resto del código no cambia, porque todos usan IEmailService
public class EmailLogService : IEmailService
{
    private readonly ILogger<EmailLogService> _logger;

    public EmailLogService(ILogger<EmailLogService> logger)
    {
        _logger = logger;
    }

    public Task EnviarAsync(string destinatario, string asunto, string cuerpo)
    {
        _logger.LogInformation("Mail para {Destinatario} | {Asunto} | {Cuerpo}", destinatario, asunto, cuerpo);
        return Task.CompletedTask;
    }
}
