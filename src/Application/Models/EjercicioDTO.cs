using Domain.Entities;

namespace Application.Models;

public class EjercicioDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string GrupoMuscular { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public static EjercicioDTO Create(Ejercicio ejercicio)
    {
        var dto = new EjercicioDTO();
        dto.Id = ejercicio.Id;
        dto.Nombre = ejercicio.Nombre;
        dto.GrupoMuscular = ejercicio.GrupoMuscular;
        dto.Descripcion = ejercicio.Descripcion;

        return dto;
    }

    public static List<EjercicioDTO> CreateList(List<Ejercicio> ejercicioList)
    {
        var dtoList = new List<EjercicioDTO>();

        foreach (var e in ejercicioList)
        {
            dtoList.Add(Create(e));
        }

        return dtoList;
    }
}
