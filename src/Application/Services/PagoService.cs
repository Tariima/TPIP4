using Application.Interfaces;
using Application.Models.Request;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class PagoService : IPagoService
{
    private readonly IPagoRepository _pagoRepository;
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly IEmailService _emailService;

    public PagoService(IPagoRepository pagoRepository, IMercadoPagoService mercadoPagoService, IEmailService emailService)
    {
        _pagoRepository = pagoRepository;
        _mercadoPagoService = mercadoPagoService;
        _emailService = emailService;
    }

    // No confiamos en el aviso: con el id consultamos la orden a MP.
    // Lo que no sirve se ignora sin error, porque si devolvemos error MP vuelve a mandar el aviso
    public async Task ProcesarNotificacion(NotificacionMercadoPagoDTO notificacion)
    {
        if (notificacion.Type != "order" || string.IsNullOrEmpty(notificacion.Data.Id))
        {
            return;
        }

        var orden = await _mercadoPagoService.GetOrdenAsync(notificacion.Data.Id);

        // si la orden no existe el aviso es falso o vino mal
        if (orden is null)
        {
            return;
        }

        // external_reference = Id de nuestro Pago
        if (!int.TryParse(orden.ReferenciaExterna, out int idPago))
        {
            return;
        }

        var pago = await _pagoRepository.GetByIdConSuscripcionAsync(idPago);

        // tiene que ser la misma orden que guardamos
        if (pago is null || pago.IdExterno != orden.Id)
        {
            return;
        }

        // MP puede mandar el mismo aviso más de una vez: solo se procesan pendientes, si no se sumaría dos veces el mes
        if (pago.Estado != EstadoPago.Pendiente)
        {
            return;
        }

        if (orden.Estado == "processed" && orden.DetalleEstado == "accredited")
        {
            pago.Estado = EstadoPago.Aprobado;
            pago.Fecha = DateTime.Now;
            pago.MedioPago = orden.MedioPago;
            ActivarSuscripcion(pago.Suscripcion);

            await _pagoRepository.UpdateAsync(pago);

            var usuario = pago.Suscripcion.UsuarioComun;
            await _emailService.EnviarAsync(
                usuario.Email,
                "Recibimos tu pago de la suscripción Pro",
                $"Hola {usuario.Nombre}, recibimos tu pago de ${pago.Monto}. Tu suscripción Pro está activa hasta el {pago.Suscripcion.FechaVencimiento:dd/MM/yyyy}.");
        }
        else if (orden.Estado == "failed" || orden.Estado == "canceled")
        {
            pago.Estado = EstadoPago.Rechazado;

            await _pagoRepository.UpdateAsync(pago);
        }
        // otros estados (creada, en proceso) todavía no definen nada, se espera el próximo aviso
    }

    // Si sigue activa se suma un mes al vencimiento (no pierde días). Si no, arranca hoy
    private void ActivarSuscripcion(Suscripcion suscripcion)
    {
        if (suscripcion.Estado == EstadoSuscripcion.Activa && suscripcion.FechaVencimiento > DateTime.Now)
        {
            suscripcion.FechaVencimiento = suscripcion.FechaVencimiento.AddMonths(1);
        }
        else
        {
            suscripcion.Estado = EstadoSuscripcion.Activa;
            suscripcion.FechaInicio = DateTime.Now;
            suscripcion.FechaVencimiento = DateTime.Now.AddMonths(1);
        }
    }
}
