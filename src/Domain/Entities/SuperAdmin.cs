namespace Domain.Entities;

public class SuperAdmin : Usuario
{
    public List<Gimnasio> Gimnasios { get; set; } = new List<Gimnasio>();
    public List<SolicitudCuentaGimnasio> Solicitudes { get; set; } = new List<SolicitudCuentaGimnasio>();
}
