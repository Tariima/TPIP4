using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Se usa para el alta y para la modificación. Las horas van como texto con formato HH:mm (ej: "08:30")
public class CreationHorarioDTO
{
    [Required(ErrorMessage = "El día de la semana es requerido.")]
    public string DiaSemana { get; set; } = string.Empty;
    [Required(ErrorMessage = "La hora de inicio es requerida.")]
    public string HoraInicio { get; set; } = string.Empty;
    [Required(ErrorMessage = "La hora de fin es requerida.")]
    public string HoraFin { get; set; } = string.Empty;
    [Required(ErrorMessage = "El cupo es requerido.")]
    public int CupoMaximo { get; set; }
}
