namespace Domain.Entities;

public class Ejercicio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string GrupoMuscular { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public List<EjercicioRutina> EjerciciosRutina { get; set; } = new List<EjercicioRutina>();
    public List<RegistroSerie> RegistrosSerie { get; set; } = new List<RegistroSerie>();
}
