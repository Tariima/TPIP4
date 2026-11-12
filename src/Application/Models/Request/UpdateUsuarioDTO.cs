using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// El email no se edita y la contraseña se cambia en /perfil/contrasena
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
