using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Datos que carga el responsable del gimnasio al pedir la cuenta
public class CreationSolicitudCuentaGimnasioDTO
{
    [Required(ErrorMessage = "El gimnasio es requerido.")]
    public int GimnasioId { get; set; }
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "El apellido es requerido.")]
    public string Apellido { get; set; } = string.Empty;
    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Contrasena { get; set; } = string.Empty;
}
