using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Credenciales que llegan en el body del endpoint de login
public class CredentialsDTO
{
    [Required(ErrorMessage = "El email es requerido.")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Contrasena { get; set; } = string.Empty;
}
