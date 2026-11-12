using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class CreationRegistroSerieDTO
{
    // Son nullables para que Required funcione: si no vienen quedan en null y se rechaza el request.
    // Con int y float comunes quedarían en 0 y, en el caso del peso, se aceptaría igual
    [Required(ErrorMessage = "El ejercicio es requerido.")]
    public int? EjercicioId { get; set; }
    [Required(ErrorMessage = "El número de serie es requerido.")]
    public int? NumeroSerie { get; set; }
    [Required(ErrorMessage = "El peso es requerido.")]
    public float? Peso { get; set; }
    [Required(ErrorMessage = "Las repeticiones son requeridas.")]
    public int? Repeticiones { get; set; }
}
