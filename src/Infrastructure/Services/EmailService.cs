using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Services;

// Manda los mails por SMTP desde la cuenta de Gmail del proyecto, con la librería MailKit.
// Los datos de la cuenta están en la sección "Email" de appsettings.json, menos la contraseña,
// que se carga con user-secrets (local) o con una variable de entorno (Azure)
public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task EnviarAsync(string destinatario, string asunto, string cuerpo)
    {
        try
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress(_config["Email:NombreRemitente"], _config["Email:Usuario"]!));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = asunto;
            mensaje.Body = new TextPart("plain") { Text = cuerpo };

            using var smtp = new SmtpClient();
            // Por defecto espera hasta 2 minutos. Si Gmail no responde, cortamos a los 10 segundos
            // para no dejar colgado el request que mandó el mail
            smtp.Timeout = 10000;

            // Gmail usa el puerto 587 con StartTls: la conexión se cifra antes de mandar la contraseña
            await smtp.ConnectAsync(_config["Email:Host"]!, int.Parse(_config["Email:Puerto"]!), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["Email:Usuario"]!, _config["Email:Password"]!);
            await smtp.SendAsync(mensaje);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Mail enviado a {Destinatario} | {Asunto}", destinatario, asunto);
        }
        catch (Exception ex)
        {
            // Si el mail no sale se registra el error y la operación que lo disparó sigue normalmente
            _logger.LogError(ex, "No se pudo mandar el mail a {Destinatario} | {Asunto}", destinatario, asunto);
        }
    }
}
