namespace Domain.Entities;

public class Entrenamiento
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int DuracionMinutos { get; set; }
    public string Observaciones { get; set; } = string.Empty;

    public int UsuarioComunId { get; set; }
    public UsuarioComun UsuarioComun { get; set; }
    public int? RutinaId { get; set; }
    public Rutina? Rutina { get; set; }
    public List<RegistroSerie> RegistrosSerie { get; set; } = new List<RegistroSerie>();
}
