using Domain.Entities;

namespace Application.Models;

public class ReservaDTO
{
    public int Id { get; set; }
    public DateTime FechaTurno { get; set; }
    public string HoraFin { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaCancelacion { get; set; }
    public int HorarioId { get; set; }
    public int GimnasioId { get; set; }
    public string NombreGimnasio { get; set; } = string.Empty;

    public static ReservaDTO Create(Reserva reserva)
    {
        var dto = new ReservaDTO();
        dto.Id = reserva.Id;
        dto.FechaTurno = reserva.FechaTurno;
        dto.HoraFin = reserva.Horario.HoraFin.ToString("HH:mm");
        dto.Estado = reserva.Estado.ToString();
        dto.FechaSolicitud = reserva.FechaSolicitud;
        dto.FechaCancelacion = reserva.FechaCancelacion;
        dto.HorarioId = reserva.HorarioId;
        dto.GimnasioId = reserva.Horario.GimnasioId;
        dto.NombreGimnasio = reserva.Horario.Gimnasio.Nombre;

        return dto;
    }

    public static List<ReservaDTO> CreateList(List<Reserva> reservaList)
    {
        var dtoList = new List<ReservaDTO>();

        foreach (var r in reservaList)
        {
            dtoList.Add(Create(r));
        }

        return dtoList;
    }
}
