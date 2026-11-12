using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Se manda completo, las series reemplazan a las anteriores
public class UpdateEntrenamientoDTO
{
    // Nullables para que funcione el [Required]
    [Required(ErrorMessage = "La fecha es requerida.")]
    public DateTime? Fecha { get; set; }
    [Required(ErrorMessage = "La duración es requerida.")]
    public int? DuracionMinutos { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public int? RutinaId { get; set; }
    public List<CreationRegistroSerieDTO> Series { get; set; } = new List<CreationRegistroSerieDTO>();
}
