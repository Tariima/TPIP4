namespace Application.Models;

// Un punto de la gráfica de un ejercicio: lo que se hizo en un día
public class ProgresoEjercicioDTO
{
    public DateTime Fecha { get; set; }
    public float PesoMaximo { get; set; }
    // Suma de peso x repeticiones de todas las series del día
    public float Volumen { get; set; }
}
