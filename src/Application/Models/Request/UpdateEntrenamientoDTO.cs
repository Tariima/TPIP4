using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Al editar se manda el entrenamiento completo: la lista de series reemplaza a la anterior
public class UpdateEntrenamientoDTO
{
    // Son nullables para que Required funcione: si no vienen quedan en null y se rechaza el request.
    // Con DateTime e int comunes quedarían en 01/01/0001 y en 0 y se aceptarían igual
    [Required(ErrorMessage = "La fecha es requerida.")]
    public DateTime? Fecha { get; set; }
    [Required(ErrorMessage = "La duración es requerida.")]
    public int? DuracionMinutos { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public int? RutinaId { get; set; }
    public List<CreationRegistroSerieDTO> Series { get; set; } = new List<CreationRegistroSerieDTO>();
}
