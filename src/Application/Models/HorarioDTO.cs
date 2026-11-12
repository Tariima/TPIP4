using Domain.Entities;

namespace Application.Models;

public class HorarioDTO
{
    public int Id { get; set; }
    public string DiaSemana { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public int CupoMaximo { get; set; }
    public bool Activo { get; set; }
    public int GimnasioId { get; set; }

    public static HorarioDTO Create(Horario horario)
    {
        var dto = new HorarioDTO();
        dto.Id = horario.Id;
        dto.DiaSemana = horario.DiaSemana;
        // la fecha guardada no importa, solo la hora
        dto.HoraInicio = horario.HoraInicio.ToString("HH:mm");
        dto.HoraFin = horario.HoraFin.ToString("HH:mm");
        dto.CupoMaximo = horario.CupoMaximo;
        dto.Activo = horario.Activo;
        dto.GimnasioId = horario.GimnasioId;

        return dto;
    }

    public static List<HorarioDTO> CreateList(List<Horario> horarioList)
    {
        var dtoList = new List<HorarioDTO>();

        foreach (var h in horarioList)
        {
            dtoList.Add(Create(h));
        }

        return dtoList;
    }
}
