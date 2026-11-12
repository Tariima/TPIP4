namespace Application.Models;

// Respuesta de /pagar
public class LinkPagoDTO
{
    public int IdPago { get; set; }
    public decimal Monto { get; set; }
    public string LinkPago { get; set; } = string.Empty;
}
