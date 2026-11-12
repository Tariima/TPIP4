using Domain.Entities;

namespace Application.Models;

public class BeneficioDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public float PorcentajeDescuento { get; set; }
    public DateTime VigenteDesde { get; set; }
    public DateTime VigenteHasta { get; set; }
    // Si hoy está dentro del período de vigencia. Le sirve al admin para ver cuáles ya vencieron o todavía no empezaron
    public bool Vigente { get; set; }

    public static BeneficioDTO Create(Beneficio beneficio)
    {
        var dto = new BeneficioDTO();
        dto.Id = beneficio.Id;
        dto.Titulo = beneficio.Titulo;
        dto.Descripcion = beneficio.Descripcion;
        dto.Tipo = beneficio.Tipo;
        dto.PorcentajeDescuento = beneficio.PorcentajeDescuento;
        dto.VigenteDesde = beneficio.VigenteDesde;
        dto.VigenteHasta = beneficio.VigenteHasta;
        dto.Vigente = beneficio.Activo
            && beneficio.VigenteDesde.Date <= DateTime.Today
            && beneficio.VigenteHasta.Date >= DateTime.Today;

        return dto;
    }

    public static List<BeneficioDTO> CreateList(List<Beneficio> beneficioList)
    {
        var dtoList = new List<BeneficioDTO>();

        foreach (var b in beneficioList)
        {
            dtoList.Add(Create(b));
        }

        return dtoList;
    }
}
