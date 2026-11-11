namespace Application.Models;

public class OcupacionHorarioDTO
{
    public int HorarioId { get; set; }
    public string DiaSemana { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public int CupoMaximo { get; set; }
    // Cuántas veces cae ese día de la semana en el período (ej: 4 jueves en el mes)
    public int CantidadTurnos { get; set; }
    public int ReservasConfirmadas { get; set; }
    public int Cancelaciones { get; set; }
    // Reservas confirmadas sobre el total de lugares que se ofrecieron (cupo x cantidad de turnos)
    public double PorcentajeOcupacion { get; set; }
}
