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

        // Si ya venció pero el job diario todavía no la actualizó, se muestra Vencida igual
        if (suscripcion.Estado == EstadoSuscripcion.Activa && suscripcion.FechaVencimiento <= DateTime.Now)
        {
            dto.Estado = EstadoSuscripcion.Vencida.ToString();
        }

        dto.FechaInicio = suscripcion.FechaInicio;
        dto.FechaVencimiento = suscripcion.FechaVencimiento;
        dto.MontoMensual = suscripcion.MontoMensual;
        // más nuevos primero
        dto.Pagos = PagoDTO.CreateList(suscripcion.Pagos.OrderByDescending(p => p.Fecha).ToList());

        return dto;
    }
}
