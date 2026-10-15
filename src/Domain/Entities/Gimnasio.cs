namespace Domain.Entities;

public class Gimnasio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaAlta { get; set; }
    public bool Activo { get; set; }

    public int SuperAdminId { get; set; }
    public SuperAdmin SuperAdmin { get; set; }
    public List<AdminGimnasio> Admins { get; set; } = new List<AdminGimnasio>();
    public List<SolicitudCuentaGimnasio> Solicitudes { get; set; } = new List<SolicitudCuentaGimnasio>();
    public List<Horario> Horarios { get; set; } = new List<Horario>();
    public List<Beneficio> Beneficios { get; set; } = new List<Beneficio>();
}
