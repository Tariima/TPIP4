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

    // días de anticipación del aviso
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

        // nunca pagó
        if (suscripcion is null)
        {
            throw new NotFoundException("No tenés una suscripción");
        }

        return SuscripcionDTO.Create(suscripcion);
    }

    // Contratar y renovar es lo mismo: pagar un mes.
    // Se activa recién cuando MP confirma el pago por el webhook
    public async Task<LinkPagoDTO> Pagar(int idUsuario)
    {
        decimal monto = _suscripcionSettings.MontoMensual;
        var suscripcion = await _suscripcionRepository.GetByUsuarioAsync(idUsuario);

        // primera vez: queda pendiente hasta que se apruebe el pago
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

        // el pago se guarda primero para tener el Id que se manda a MP
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
            // si MP falló (ya con los reintentos) el pago queda rechazado y el error sigue para arriba
            pago.Estado = EstadoPago.Rechazado;
            await _pagoRepository.UpdateAsync(pago);
            throw;
        }
    }

    // Lo llama una vez por día VencimientoSuscripcionesService
    public async Task ProcesarVencimientos()
    {
        await AvisarProximasAVencer();
        await VencerSuscripciones();
    }

    // Las que vencen en 3 días. Se busca solo ese día (00:00 a 00:00) para que el aviso salga una sola vez
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

    // Pasa a Vencida las que ya vencieron. Al cambiar de estado no vuelven a entrar
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
