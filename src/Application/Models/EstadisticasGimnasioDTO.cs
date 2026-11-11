namespace Application.Models;

public class EstadisticasGimnasioDTO
{
    public DateTime Desde { get; set; }
    public DateTime Hasta { get; set; }
    public int TotalReservas { get; set; }
    public int Confirmadas { get; set; }
    public int Canceladas { get; set; }
    public double PorcentajeCancelacion { get; set; }
    public List<OcupacionHorarioDTO> OcupacionPorHorario { get; set; } = new List<OcupacionHorarioDTO>();
}
