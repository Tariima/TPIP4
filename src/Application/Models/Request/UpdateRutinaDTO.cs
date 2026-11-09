using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Al editar se manda la rutina completa: la lista de ejercicios reemplaza a la anterior
public class UpdateRutinaDTO
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public List<CreationEjercicioRutinaDTO> Ejercicios { get; set; } = new List<CreationEjercicioRutinaDTO>();
}
