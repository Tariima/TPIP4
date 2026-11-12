using Domain.Entities;

namespace Application.Models;

public class EntrenamientoDTO
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int DuracionMinutos { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public int? RutinaId { get; set; }
    public string? NombreRutina { get; set; }
    public List<RegistroSerieDTO> Series { get; set; } = new List<RegistroSerieDTO>();

    public static EntrenamientoDTO Create(Entrenamiento entrenamiento)
    {
        var dto = new EntrenamientoDTO();
        dto.Id = entrenamiento.Id;
        dto.Fecha = entrenamiento.Fecha;
        dto.DuracionMinutos = entrenamiento.DuracionMinutos;
        dto.Observaciones = entrenamiento.Observaciones;
        dto.RutinaId = entrenamiento.RutinaId;
        // null si no se usó una rutina
        dto.NombreRutina = entrenamiento.Rutina?.Nombre;
        // ordenadas por ejercicio y número de serie
        dto.Series = RegistroSerieDTO.CreateList(entrenamiento.RegistrosSerie
            .OrderBy(rs => rs.EjercicioId)
            .ThenBy(rs => rs.NumeroSerie)
            .ToList());

        return dto;
    }

    public static List<EntrenamientoDTO> CreateList(List<Entrenamiento> entrenamientoList)
    {
        var dtoList = new List<EntrenamientoDTO>();

        foreach (var e in entrenamientoList)
        {
            dtoList.Add(Create(e));
        }

        return dtoList;
    }
}
