using Domain.Entities;

namespace Application.Models;

public class SolicitudCuentaGimnasioDTO
{
    public int Id { get; set; }
    public string NombreSolicitante { get; set; } = string.Empty;
    public string EmailSolicitante { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaResolucion { get; set; }
    public int GimnasioId { get; set; }
    public string NombreGimnasio { get; set; } = string.Empty;

    public static SolicitudCuentaGimnasioDTO Create(SolicitudCuentaGimnasio solicitud)
    {
        var dto = new SolicitudCuentaGimnasioDTO();
        dto.Id = solicitud.Id;
        dto.NombreSolicitante = solicitud.NombreSolicitante;
        dto.EmailSolicitante = solicitud.EmailSolicitante;
        dto.FechaSolicitud = solicitud.FechaSolicitud;
        dto.Estado = solicitud.Estado.ToString();
        dto.FechaResolucion = solicitud.FechaResolucion;
        dto.GimnasioId = solicitud.GimnasioId;
        dto.NombreGimnasio = solicitud.Gimnasio.Nombre;

        return dto;
    }

    public static List<SolicitudCuentaGimnasioDTO> CreateList(List<SolicitudCuentaGimnasio> solicitudList)
    {
        var dtoList = new List<SolicitudCuentaGimnasioDTO>();

        foreach (var s in solicitudList)
        {
            dtoList.Add(Create(s));
        }

        return dtoList;
    }
}
