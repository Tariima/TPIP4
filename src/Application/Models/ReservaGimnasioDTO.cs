using Domain.Entities;

namespace Application.Models;

// Reserva como la ve el admin del gimnasio: con el usuario que la hizo
public class ReservaGimnasioDTO
{
    public int Id { get; set; }
    public DateTime FechaTurno { get; set; }
    public string HoraFin { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaCancelacion { get; set; }
    public int HorarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;

    public static ReservaGimnasioDTO Create(Reserva reserva)
    {
        var dto = new ReservaGimnasioDTO();
        dto.Id = reserva.Id;
        dto.FechaTurno = reserva.FechaTurno;
        dto.HoraFin = reserva.Horario.HoraFin.ToString("HH:mm");
        dto.Estado = reserva.Estado.ToString();
        dto.FechaCancelacion = reserva.FechaCancelacion;
        dto.HorarioId = reserva.HorarioId;
        dto.NombreUsuario = $"{reserva.UsuarioComun.Nombre} {reserva.UsuarioComun.Apellido}";

        return dto;
    }

    public static List<ReservaGimnasioDTO> CreateList(List<Reserva> reservaList)
    {
        var dtoList = new List<ReservaGimnasioDTO>();

        foreach (var r in reservaList)
        {
            dtoList.Add(Create(r));
        }

        return dtoList;
    }
}
