using Domain.Entities;

namespace Application.Models;

public class RegistroSerieDTO
{
    public int EjercicioId { get; set; }
    public string NombreEjercicio { get; set; } = string.Empty;
    public int NumeroSerie { get; set; }
    public float Peso { get; set; }
    public int Repeticiones { get; set; }

    public static RegistroSerieDTO Create(RegistroSerie registroSerie)
    {
        var dto = new RegistroSerieDTO();
        dto.EjercicioId = registroSerie.EjercicioId;
        dto.NombreEjercicio = registroSerie.Ejercicio.Nombre;
        dto.NumeroSerie = registroSerie.NumeroSerie;
        dto.Peso = registroSerie.Peso;
        dto.Repeticiones = registroSerie.Repeticiones;

        return dto;
    }

    public static List<RegistroSerieDTO> CreateList(List<RegistroSerie> registroSerieList)
    {
        var dtoList = new List<RegistroSerieDTO>();

        foreach (var rs in registroSerieList)
        {
            dtoList.Add(Create(rs));
        }

        return dtoList;
    }
}
