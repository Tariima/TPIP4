using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Se manda completa, los ejercicios reemplazan a los anteriores
public class UpdateRutinaDTO
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public List<CreationEjercicioRutinaDTO> Ejercicios { get; set; } = new List<CreationEjercicioRutinaDTO>();
}
