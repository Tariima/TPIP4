using Domain.Enums;

namespace Domain.Entities;

public class Reserva
{
    public int Id { get; set; }
    public DateTime FechaTurno { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaCancelacion { get; set; }
    public EstadoReserva Estado { get; set; }

    public int UsuarioComunId { get; set; }
    public UsuarioComun UsuarioComun { get; set; }
    public int HorarioId { get; set; }
    public Horario Horario { get; set; }
}
