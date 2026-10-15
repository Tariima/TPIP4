using Domain.Enums;

namespace Domain.Entities;

public class Suscripcion
{
    public int Id { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal MontoMensual { get; set; }
    public EstadoSuscripcion Estado { get; set; }

    public int UsuarioComunId { get; set; }
    public UsuarioComun UsuarioComun { get; set; }
    public List<Pago> Pagos { get; set; } = new List<Pago>();
}
