namespace Domain.Entities;

public class EjercicioRutina
{
    public int Id { get; set; }
    public int Orden { get; set; }
    public int SeriesObjetivo { get; set; }
    public int RepeticionesObjetivo { get; set; }

    public int RutinaId { get; set; }
    public Rutina Rutina { get; set; }
    public int EjercicioId { get; set; }
    public Ejercicio Ejercicio { get; set; }
}
