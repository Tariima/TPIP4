using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Se usa para el alta y para la modificación
public class CreationBeneficioDTO
{
    [Required(ErrorMessage = "El título es requerido.")]
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    // Texto libre, ej: "Descuento en cuota", "Clase gratis"
    public string Tipo { get; set; } = string.Empty;
    // Son nullables para que Required funcione: si no vienen quedan en null y se rechaza el request.
    // Con float y DateTime comunes quedarían en 0 y en 01/01/0001 y se aceptarían igual
    [Required(ErrorMessage = "El porcentaje de descuento es requerido.")]
    public float? PorcentajeDescuento { get; set; }
    [Required(ErrorMessage = "La fecha de inicio de vigencia es requerida.")]
    public DateTime? VigenteDesde { get; set; }
    [Required(ErrorMessage = "La fecha de fin de vigencia es requerida.")]
    public DateTime? VigenteHasta { get; set; }
}
