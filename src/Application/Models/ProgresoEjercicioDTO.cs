namespace Application.Models;

// Un punto del gráfico (un día)
public class ProgresoEjercicioDTO
{
    public DateTime Fecha { get; set; }
    public float PesoMaximo { get; set; }
    // suma de peso * reps del día
    public float Volumen { get; set; }
}
