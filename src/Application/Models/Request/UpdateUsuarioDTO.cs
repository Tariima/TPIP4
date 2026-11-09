using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Datos que se pueden editar del perfil. El email no se cambia y la contraseña tiene su propio endpoint
public class UpdateUsuarioDTO
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "El apellido es requerido.")]
    public string Apellido { get; set; } = string.Empty;
    [Required(ErrorMessage = "La fecha de nacimiento es requerida.")]
    public DateTime FechaNacimiento { get; set; }
    [Required(ErrorMessage = "El peso es requerido.")]
    public float Peso { get; set; }
    [Required(ErrorMessage = "La altura es requerida.")]
    public float Altura { get; set; }
}
