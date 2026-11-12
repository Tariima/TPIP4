using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class CreationEntrenamientoDTO
{
    // Nullables para que funcione el [Required] (si no, una fecha que no viene queda en 01/01/0001)
    [Required(ErrorMessage = "La fecha es requerida.")]
    public DateTime? Fecha { get; set; }
    [Required(ErrorMessage = "La duración es requerida.")]
    public int? DuracionMinutos { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    // opcional
    public int? RutinaId { get; set; }
    public List<CreationRegistroSerieDTO> Series { get; set; } = new List<CreationRegistroSerieDTO>();
}
