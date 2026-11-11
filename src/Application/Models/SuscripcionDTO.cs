using Domain.Entities;
using Domain.Enums;

namespace Application.Models;

public class SuscripcionDTO
{
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal MontoMensual { get; set; }
    public List<PagoDTO> Pagos { get; set; } = new List<PagoDTO>();

    public static SuscripcionDTO Create(Suscripcion suscripcion)
    {
        var dto = new SuscripcionDTO();
        dto.Estado = suscripcion.Estado.ToString();

        // Si ya pasó la fecha pero el proceso diario todavía no la marcó como vencida,
        // se muestra vencida igual. Es la misma regla que se usa para saber si el usuario es Pro
        if (suscripcion.Estado == EstadoSuscripcion.Activa && suscripcion.FechaVencimiento <= DateTime.Now)
        {
            dto.Estado = EstadoSuscripcion.Vencida.ToString();
        }

        dto.FechaInicio = suscripcion.FechaInicio;
        dto.FechaVencimiento = suscripcion.FechaVencimiento;
        dto.MontoMensual = suscripcion.MontoMensual;
        // Los pagos del más nuevo al más viejo
        dto.Pagos = PagoDTO.CreateList(suscripcion.Pagos.OrderByDescending(p => p.Fecha).ToList());

        return dto;
    }
}
