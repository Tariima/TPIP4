using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class CreationRutinaDTO
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public List<CreationEjercicioRutinaDTO> Ejercicios { get; set; } = new List<CreationEjercicioRutinaDTO>();
}