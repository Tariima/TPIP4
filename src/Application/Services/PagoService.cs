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

    // Mercado Pago avisa que una orden cambió. No confiamos en el aviso: con el id
    // consultamos la orden a Mercado Pago y recién ahí actualizamos el pago y la suscripción.
    // Los casos que no nos sirven se ignoran sin error, porque si respondemos con error
    // Mercado Pago sigue reintentando el mismo aviso
    public async Task ProcesarNotificacion(NotificacionMercadoPagoDTO notificacion)
    {
        if (notificacion.Type != "order" || string.IsNullOrEmpty(notificacion.Data.Id))
        {
            return;
        }

        var orden = await _mercadoPagoService.GetOrdenAsync(notificacion.Data.Id);

        // Si la orden no existe en Mercado Pago el aviso es falso o está mal
        if (orden is null)
        {
            return;
        }

        // En external_reference mandamos el Id de nuestro pago al crear la orden
        if (!int.TryParse(orden.ReferenciaExterna, out int idPago))
        {
            return;
        }

        var pago = await _pagoRepository.GetByIdConSuscripcionAsync(idPago);

        // La orden tiene que ser la misma que guardamos para ese pago
        if (pago is null || pago.IdExterno != orden.Id)
        {
            return;
        }

        // Si el pago ya se procesó (Mercado Pago puede mandar el mismo aviso más de una vez)
        // no se hace nada, así no se suma dos veces el mes
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
        // Cualquier otro estado (creada, en proceso) todavía no está definido:
        // se espera el próximo aviso de Mercado Pago
    }

    // Si la suscripción sigue activa se le suma un mes al vencimiento (renovación, no pierde días).
    // Si estaba pendiente o vencida arranca hoy y vence en un mes
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
