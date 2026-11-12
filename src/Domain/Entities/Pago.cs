using Domain.Enums;

namespace Domain.Entities;

public class Pago
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string MedioPago { get; set; } = string.Empty;
    public EstadoPago Estado { get; set; }
    // Id de la orden en MP, null hasta que se crea
    public string? IdExterno { get; set; }

    public int SuscripcionId { get; set; }
    public Suscripcion Suscripcion { get; set; }
}
