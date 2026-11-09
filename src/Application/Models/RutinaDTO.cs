using Domain.Entities;

namespace Application.Models;

public class RutinaDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public bool Activa { get; set; }
    public List<EjercicioRutinaDTO> Ejercicios { get; set; } = new List<EjercicioRutinaDTO>();

    public static RutinaDTO Create(Rutina rutina)
    {
        var dto = new RutinaDTO();
        dto.Id = rutina.Id;
        dto.Nombre = rutina.Nombre;
        dto.Descripcion = rutina.Descripcion;
        dto.FechaCreacion = rutina.FechaCreacion;
        dto.Activa = rutina.Activa;
        dto.Ejercicios = EjercicioRutinaDTO.CreateList(rutina.EjerciciosRutina.OrderBy(er => er.Orden).ToList());

        return dto;
    }

    public static List<RutinaDTO> CreateList(List<Rutina> rutinaList)
    {
        var dtoList = new List<RutinaDTO>();

        foreach (var r in rutinaList)
        {
            dtoList.Add(Create(r));
        }

        return dtoList;
    }
}