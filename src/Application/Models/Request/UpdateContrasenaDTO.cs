using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class UpdateContrasenaDTO
{
    [Required(ErrorMessage = "La contraseña actual es requerida.")]
    public string ContrasenaActual { get; set; } = string.Empty;
    [Required(ErrorMessage = "La contraseña nueva es requerida.")]
    public string ContrasenaNueva { get; set; } = string.Empty;
}
