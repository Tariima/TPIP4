namespace Application.Models;

// Una semana (lunes a domingo) del gráfico semanal
public class ResumenSemanalDTO
{
    public DateTime InicioSemana { get; set; }
    public int CantidadEntrenamientos { get; set; }
    public int MinutosTotales { get; set; }
    public float VolumenTotal { get; set; }
}
