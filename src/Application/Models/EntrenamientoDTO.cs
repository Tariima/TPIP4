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
        // Si el entrenamiento no se basó en una rutina queda en null
        dto.NombreRutina = entrenamiento.Rutina?.Nombre;
        // Las series se muestran agrupadas por ejercicio y en el orden en que se hicieron
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
