namespace Application.Models;

// Lo que usamos de una orden de MP
public class OrdenMercadoPagoDTO
{
    public string Id { get; set; } = string.Empty;
    // solo viene al crear la orden
    public string CheckoutUrl { get; set; } = string.Empty;
    // processed / failed / canceled
    public string Estado { get; set; } = string.Empty;
    // accredited = se acreditó
    public string DetalleEstado { get; set; } = string.Empty;
    // el Id de nuestro Pago
    public string ReferenciaExterna { get; set; } = string.Empty;
    public string MedioPago { get; set; } = string.Empty;
}
