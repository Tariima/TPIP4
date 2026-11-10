using Domain.Entities;

namespace Application.Models;

public class GimnasioDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaAlta { get; set; }
    public bool Activo { get; set; }

    public static GimnasioDTO Create(Gimnasio gimnasio)
    {
        var dto = new GimnasioDTO();
        dto.Id = gimnasio.Id;
        dto.Nombre = gimnasio.Nombre;
        dto.Direccion = gimnasio.Direccion;
        dto.Telefono = gimnasio.Telefono;
        dto.Email = gimnasio.Email;
        dto.FechaAlta = gimnasio.FechaAlta;
        dto.Activo = gimnasio.Activo;

        return dto;
    }

    public static List<GimnasioDTO> CreateList(List<Gimnasio> gimnasioList)
    {
        var dtoList = new List<GimnasioDTO>();

        foreach (var g in gimnasioList)
        {
            dtoList.Add(Create(g));
        }

        return dtoList;
    }
}
