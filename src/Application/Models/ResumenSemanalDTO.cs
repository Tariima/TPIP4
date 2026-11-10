namespace Application.Models;

// Un punto de la gráfica semanal. La semana va de lunes a domingo
public class ResumenSemanalDTO
{
    public DateTime InicioSemana { get; set; }
    public int CantidadEntrenamientos { get; set; }
    public int MinutosTotales { get; set; }
    public float VolumenTotal { get; set; }
}
