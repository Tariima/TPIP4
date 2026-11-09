using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Se usa para el alta y para la modificación, porque los datos que se cargan son los mismos
public class CreationGimnasioDTO
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "La dirección es requerida.")]
    public string Direccion { get; set; } = string.Empty;
    [Required(ErrorMessage = "El teléfono es requerido.")]
    public string Telefono { get; set; } = string.Empty;
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    public string Email { get; set; } = string.Empty;
}
