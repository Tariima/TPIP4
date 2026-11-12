using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class CreationRegistroSerieDTO
{
    // Nullables para que funcione el [Required] (un peso que no viene quedaría en 0)
    [Required(ErrorMessage = "El ejercicio es requerido.")]
    public int? EjercicioId { get; set; }
    [Required(ErrorMessage = "El número de serie es requerido.")]
    public int? NumeroSerie { get; set; }
    [Required(ErrorMessage = "El peso es requerido.")]
    public float? Peso { get; set; }
    [Required(ErrorMessage = "Las repeticiones son requeridas.")]
    public int? Repeticiones { get; set; }
}
