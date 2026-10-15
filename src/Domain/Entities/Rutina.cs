namespace Domain.Entities;

public class Rutina
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public bool Activa { get; set; }

    public int UsuarioComunId { get; set; }
    public UsuarioComun UsuarioComun { get; set; }
    public List<EjercicioRutina> EjerciciosRutina { get; set; } = new List<EjercicioRutina>();
    public List<Entrenamiento> Entrenamientos { get; set; } = new List<Entrenamiento>();
}
