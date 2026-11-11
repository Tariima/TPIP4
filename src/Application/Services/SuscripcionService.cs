using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class SuscripcionService : ISuscripcionService
{
    private readonly ISuscripcionRepository _suscripcionRepository;
    private readonly IPagoRepository _pagoRepository;
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly SuscripcionSettings _suscripcionSettings;
    private readonly IEmailService _emailService;

    // Cuántos días antes del vencimiento se manda el aviso
    private const int DiasAvisoPrevio = 3;

    public SuscripcionService(ISuscripcionRepository suscripcionRepository, IPagoRepository pagoRepository, IMercadoPagoService mercadoPagoService, SuscripcionSettings suscripcionSettings, IEmailService emailService)
    {
        _suscripcionRepository = suscripcionRepository;
        _pagoRepository = pagoRepository;
        _mercadoPagoService = mercadoPagoService;
        _suscripcionSettings = suscripcionSettings;
        _emailService = emailService;
    }

    public async Task<SuscripcionDTO> GetMiSuscripcion(int idUsuario)
    {
        var suscripcion = await _suscripcionRepository.GetByUsuarioAsync(idUsuario);

        // Si nunca pagó no tiene suscripción
        if (suscripcion is null)
        {
            throw new NotFoundException("No tenés una suscripción");
        }

        return SuscripcionDTO.Create(suscripcion);
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

    // Lo ejecuta una vez por día el BackgroundService (VencimientoSuscripcionesService)
    public async Task ProcesarVencimientos()
    {
        await AvisarProximasAVencer();
        await VencerSuscripciones();
    }

    // Avisa a las que vencen dentro de 3 días. Se busca solo ese día (de 00:00 a 00:00 del siguiente),
    // así cada suscripción entra una sola vez y el aviso no se repite en las corridas de otros días
    private async Task AvisarProximasAVencer()
    {
        var desde = DateTime.Today.AddDays(DiasAvisoPrevio);
        var hasta = desde.AddDays(1);

        var suscripciones = await _suscripcionRepository.GetActivasQueVencenEntreAsync(desde, hasta);

        foreach (var s in suscripciones)
        {
            await _emailService.EnviarAsync(
                s.UsuarioComun.Email,
                "Tu suscripción Pro está por vencer",
                $"Hola {s.UsuarioComun.Nombre}, tu suscripción Pro vence el {s.FechaVencimiento:dd/MM/yyyy}. Renovala para no perder los beneficios.");
        }
    }

    // Pasa a vencidas las que ya pasaron su fecha. Al dejar de estar activas,
    // en la próxima corrida ya no aparecen y el mail tampoco se repite
    private async Task VencerSuscripciones()
    {
        var suscripciones = await _suscripcionRepository.GetActivasQueVencenEntreAsync(DateTime.MinValue, DateTime.Now);

        foreach (var s in suscripciones)
        {
            s.Estado = EstadoSuscripcion.Vencida;
            await _suscripcionRepository.UpdateAsync(s);

            await _emailService.EnviarAsync(
                s.UsuarioComun.Email,
                "Tu suscripción Pro venció",
                $"Hola {s.UsuarioComun.Nombre}, tu suscripción Pro venció el {s.FechaVencimiento:dd/MM/yyyy}. Podés renovarla cuando quieras desde la app.");
        }
    }
}
