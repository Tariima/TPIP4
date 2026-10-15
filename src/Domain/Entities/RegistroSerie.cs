namespace Domain.Entities;

public class RegistroSerie
{
    public int Id { get; set; }
    public int NumeroSerie { get; set; }
    public float Peso { get; set; }
    public int Repeticiones { get; set; }

    public int EntrenamientoId { get; set; }
    public Entrenamiento Entrenamiento { get; set; }
    public int EjercicioId { get; set; }
    public Ejercicio Ejercicio { get; set; }
}
