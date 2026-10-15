using Domain.Enums;

namespace Domain.Entities;

public class SolicitudCuentaGimnasio
{
    public int Id { get; set; }
    public string NombreSolicitante { get; set; } = string.Empty;
    public string EmailSolicitante { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public EstadoSolicitud Estado { get; set; }
    public DateTime? FechaResolucion { get; set; }

    public int GimnasioId { get; set; }
    public Gimnasio Gimnasio { get; set; }
    public int? SuperAdminId { get; set; }
    public SuperAdmin? SuperAdmin { get; set; }
}
