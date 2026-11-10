using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class CreationEntrenamientoDTO
{
    [Required(ErrorMessage = "La fecha es requerida.")]
    public DateTime Fecha { get; set; }
    [Required(ErrorMessage = "La duración es requerida.")]
    public int DuracionMinutos { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    // Opcional: solo se manda si el entrenamiento se basó en una rutina
    public int? RutinaId { get; set; }
    public List<CreationRegistroSerieDTO> Series { get; set; } = new List<CreationRegistroSerieDTO>();
}
