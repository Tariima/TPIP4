using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class SuscripcionService : ISuscripcionService
{
    private readonly ISuscripcionRepository _suscripcionRepository;
    private readonly IPagoRepository _pagoRepository;
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly SuscripcionSettings _suscripcionSettings;

    public SuscripcionService(ISuscripcionRepository suscripcionRepository, IPagoRepository pagoRepository, IMercadoPagoService mercadoPagoService, SuscripcionSettings suscripcionSettings)
    {
        _suscripcionRepository = suscripcionRepository;
        _pagoRepository = pagoRepository;
        _mercadoPagoService = mercadoPagoService;
        _suscripcionSettings = suscripcionSettings;
    }

    // Sirve para contratar y para renovar: en los dos casos se paga un mes.
    // La suscripción recién se activa (o se extiende) cuando Mercado Pago confirma el pago
    public async Task<LinkPagoDTO> Pagar(int idUsuario)
    {
        decimal monto = _suscripcionSettings.MontoMensual;
        var suscripcion = await _suscripcionRepository.GetByUsuarioAsync(idUsuario);

        // La primera vez que paga se crea la suscripción, que queda pendiente hasta que se apruebe el pago
        if (suscripcion is null)
        {
            suscripcion = new Suscripcion();
            suscripcion.UsuarioComunId = idUsuario;
            suscripcion.Estado = EstadoSuscripcion.Pendiente;
            suscripcion.MontoMensual = monto;
            suscripcion.FechaInicio = DateTime.Now;
            suscripcion.FechaVencimiento = DateTime.Now;

            suscripcion = await _suscripcionRepository.CreateAsync(suscripcion);
        }

        // Primero se guarda el pago para tener su Id, que se le manda a Mercado Pago
        var pago = new Pago();
        pago.Fecha = DateTime.Now;
        pago.Monto = monto;
        pago.Estado = EstadoPago.Pendiente;
        pago.SuscripcionId = suscripcion.Id;

        pago = await _pagoRepository.CreateAsync(pago);

        try
        {
            var orden = await _mercadoPagoService.CrearOrdenAsync(pago.Id, monto, "Suscripción Pro - 1 mes");

            pago.IdExterno = orden.Id;
            await _pagoRepository.UpdateAsync(pago);

            var dto = new LinkPagoDTO();
            dto.IdPago = pago.Id;
            dto.Monto = monto;
            dto.LinkPago = orden.CheckoutUrl;

            return dto;
        }
        catch
        {
            // Si Mercado Pago falló (aun después de los reintentos) el pago no se va a poder hacer,
            // así que se marca como rechazado y se deja seguir el error para que el usuario lo vea
            pago.Estado = EstadoPago.Rechazado;
            await _pagoRepository.UpdateAsync(pago);
            throw;
        }
    }
}
