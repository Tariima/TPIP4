using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class CreationReservaDTO
{
    // [Required] en un int no sirve (queda en 0), por eso el Range
    [Required(ErrorMessage = "El horario es requerido.")]
    [Range(1, int.MaxValue, ErrorMessage = "El horario es requerido.")]
    public int HorarioId { get; set; }
    // solo se usa el día, la hora sale del horario
    [Required(ErrorMessage = "La fecha es requerida.")]
    public DateTime Fecha { get; set; }
}
