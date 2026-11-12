using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

// Sirve para alta y modificación
public class CreationBeneficioDTO
{
    [Required(ErrorMessage = "El título es requerido.")]
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    // texto libre, ej: "Clase gratis"
    public string Tipo { get; set; } = string.Empty;
    // Nullables a propósito: con float/DateTime el [Required] no detecta si faltan (quedan en 0 o 01/01/0001)
    [Required(ErrorMessage = "El porcentaje de descuento es requerido.")]
    public float? PorcentajeDescuento { get; set; }
    [Required(ErrorMessage = "La fecha de inicio de vigencia es requerida.")]
    public DateTime? VigenteDesde { get; set; }
    [Required(ErrorMessage = "La fecha de fin de vigencia es requerida.")]
    public DateTime? VigenteHasta { get; set; }
}
