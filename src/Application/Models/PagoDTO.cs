using Domain.Entities;

namespace Application.Models;

public class PagoDTO
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string MedioPago { get; set; } = string.Empty;
    // Se devuelve como texto (Pendiente, Aprobado, Rechazado) y no como número
    public string Estado { get; set; } = string.Empty;

    public static PagoDTO Create(Pago pago)
    {
        var dto = new PagoDTO();
        dto.Id = pago.Id;
        dto.Fecha = pago.Fecha;
        dto.Monto = pago.Monto;
        dto.MedioPago = pago.MedioPago;
        dto.Estado = pago.Estado.ToString();

        return dto;
    }

    public static List<PagoDTO> CreateList(List<Pago> pagoList)
    {
        var dtoList = new List<PagoDTO>();

        foreach (var p in pagoList)
        {
            dtoList.Add(Create(p));
        }

        return dtoList;
    }
}
