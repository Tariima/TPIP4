using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class CredentialsDTO
{
    [Required(ErrorMessage = "El email es requerido.")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Contrasena { get; set; } = string.Empty;
}
