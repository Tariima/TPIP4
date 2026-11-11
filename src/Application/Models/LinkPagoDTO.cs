namespace Application.Models;

// Lo que se le devuelve al usuario cuando quiere pagar: el link de Mercado Pago
public class LinkPagoDTO
{
    public int IdPago { get; set; }
    public decimal Monto { get; set; }
    public string LinkPago { get; set; } = string.Empty;
}
