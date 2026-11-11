using System.ComponentModel.DataAnnotations;

namespace Application.Models.Request;

public class CreationReservaDTO
{
    // Con un int, Required no alcanza: si no viene queda en 0. Por eso se pide que sea mayor a 0
    [Required(ErrorMessage = "El horario es requerido.")]
    [Range(1, int.MaxValue, ErrorMessage = "El horario es requerido.")]
    public int HorarioId { get; set; }
    // Solo importa el día (ej: "2026-11-16"), la hora sale del horario
    [Required(ErrorMessage = "La fecha es requerida.")]
    public DateTime Fecha { get; set; }
}
