namespace Domain.Entities;

public class UsuarioComun : Usuario
{
    public DateTime FechaNacimiento { get; set; }
    public float Peso { get; set; }
    public float Altura { get; set; }
    public int PuntajeTotal { get; set; }

    public List<Rutina> Rutinas { get; set; } = new List<Rutina>();
    public List<Entrenamiento> Entrenamientos { get; set; } = new List<Entrenamiento>();
    public List<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
    public List<Reserva> Reservas { get; set; } = new List<Reserva>();
}
