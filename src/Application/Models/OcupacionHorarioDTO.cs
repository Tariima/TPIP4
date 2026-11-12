namespace Application.Models;

public class OcupacionHorarioDTO
{
    public int HorarioId { get; set; }
    public string DiaSemana { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public int CupoMaximo { get; set; }
    // ej: 4 jueves en el mes
    public int CantidadTurnos { get; set; }
    public int ReservasConfirmadas { get; set; }
    public int Cancelaciones { get; set; }
    // confirmadas / (cupo * turnos)
    public double PorcentajeOcupacion { get; set; }
}
