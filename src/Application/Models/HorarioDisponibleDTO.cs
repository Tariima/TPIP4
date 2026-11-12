using Domain.Entities;

namespace Application.Models;

// Horario reservable en una fecha, con los lugares libres
public class HorarioDisponibleDTO
{
    public int HorarioId { get; set; }
    public string DiaSemana { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public int CupoMaximo { get; set; }
    public int CupoDisponible { get; set; }

    public static HorarioDisponibleDTO Create(Horario horario, int cupoDisponible)
    {
        var dto = new HorarioDisponibleDTO();
        dto.HorarioId = horario.Id;
        dto.DiaSemana = horario.DiaSemana;
        dto.HoraInicio = horario.HoraInicio.ToString("HH:mm");
        dto.HoraFin = horario.HoraFin.ToString("HH:mm");
        dto.CupoMaximo = horario.CupoMaximo;
        dto.CupoDisponible = cupoDisponible;

        return dto;
    }
}
