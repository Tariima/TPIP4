using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Services;

// SMTP con Gmail usando MailKit. La config está en la sección "Email" del appsettings,
// menos la contraseña, que va en user-secrets o en las variables de entorno de Azure
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
            // el default son 2 min, lo bajamos a 10 seg para no colgar el request
            smtp.Timeout = 10000;

            // 587 + StartTls: se cifra antes de mandar la contraseña
            await smtp.ConnectAsync(_config["Email:Host"]!, int.Parse(_config["Email:Puerto"]!), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["Email:Usuario"]!, _config["Email:Password"]!);
            await smtp.SendAsync(mensaje);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Mail enviado a {Destinatario} | {Asunto}", destinatario, asunto);
        }
        catch (Exception ex)
        {
            // si falla solo se loguea, lo que mandó el mail sigue normal
            _logger.LogError(ex, "No se pudo mandar el mail a {Destinatario} | {Asunto}", destinatario, asunto);
        }
    }
}
