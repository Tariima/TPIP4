using Domain.Entities;

namespace Application.Models;

public class EjercicioRutinaDTO
{
    public int EjercicioId { get; set; }
    public string NombreEjercicio { get; set; } = string.Empty;
    public int Orden { get; set; }
    public int SeriesObjetivo { get; set; }
    public int RepeticionesObjetivo { get; set; }

    public static EjercicioRutinaDTO Create(EjercicioRutina ejercicioRutina)
    {
        var dto = new EjercicioRutinaDTO();
        dto.EjercicioId = ejercicioRutina.EjercicioId;
        dto.NombreEjercicio = ejercicioRutina.Ejercicio.Nombre;
        dto.Orden = ejercicioRutina.Orden;
        dto.SeriesObjetivo = ejercicioRutina.SeriesObjetivo;
        dto.RepeticionesObjetivo = ejercicioRutina.RepeticionesObjetivo;

        return dto;
    }

    public static List<EjercicioRutinaDTO> CreateList(List<EjercicioRutina> ejercicioRutinaList)
    {
        var dtoList = new List<EjercicioRutinaDTO>();

        foreach (var er in ejercicioRutinaList)
        {
            dtoList.Add(Create(er));
        }

        return dtoList;
    }
}