using Domain.Enums;

namespace Domain.Entities;

public class Pago
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string MedioPago { get; set; } = string.Empty;
    public EstadoPago Estado { get; set; }

    public int SuscripcionId { get; set; }
    public Suscripcion Suscripcion { get; set; }
}
