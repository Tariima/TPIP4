using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Al editar se manda el entrenamiento completo: la lista de series reemplaza a la anterior
public class UpdateEntrenamientoDTO
{
    [Required(ErrorMessage = "La fecha es requerida.")]
    public DateTime Fecha { get; set; }
    [Required(ErrorMessage = "La duración es requerida.")]
    public int DuracionMinutos { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public int? RutinaId { get; set; }
    public List<CreationRegistroSerieDTO> Series { get; set; } = new List<CreationRegistroSerieDTO>();
}
