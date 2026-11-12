using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Sirve para alta y modificación
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
