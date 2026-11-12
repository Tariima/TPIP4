using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class CreationUsuarioDTO
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "El apellido es requerido.")]
    public string Apellido { get; set; } = string.Empty;
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Contrasena { get; set; } = string.Empty;
    // Es nullable para que Required funcione: si no viene queda en null y se rechaza el request.
    // Con DateTime común quedaría en 01/01/0001 y se aceptaría igual
    [Required(ErrorMessage = "La fecha de nacimiento es requerida.")]
    public DateTime? FechaNacimiento { get; set; }
    [Required(ErrorMessage = "El peso es requerido.")]
    public float Peso { get; set; }
    [Required(ErrorMessage = "La altura es requerida.")]
    public float Altura { get; set; }
}
