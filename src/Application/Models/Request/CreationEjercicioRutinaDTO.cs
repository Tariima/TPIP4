using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class CreationEjercicioRutinaDTO
{
    [Required(ErrorMessage = "El ejercicio es requerido.")]
    public int EjercicioId { get; set; }
    [Required(ErrorMessage = "El orden es requerido.")]
    public int Orden { get; set; }
    [Required(ErrorMessage = "Las series objetivo son requeridas.")]
    public int SeriesObjetivo { get; set; }
    [Required(ErrorMessage = "Las repeticiones objetivo son requeridas.")]
    public int RepeticionesObjetivo { get; set; }
}