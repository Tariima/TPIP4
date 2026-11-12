namespace Application.Models;

// Para la lista de beneficios que ve el usuario Pro
public class GimnasioConBeneficiosDTO
{
    public int GimnasioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public List<BeneficioDTO> Beneficios { get; set; } = new List<BeneficioDTO>();
}
