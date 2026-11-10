namespace Application.Models;

// Los datos de una orden de Mercado Pago que nos interesan, ya traducidos a nuestros nombres
public class OrdenMercadoPagoDTO
{
    public string Id { get; set; } = string.Empty;
    // Link al que se manda al usuario para que pague (solo viene al crear la orden)
    public string CheckoutUrl { get; set; } = string.Empty;
    // Ej: "processed" cuando se pagó, "failed" si se rechazó, "canceled" si se canceló o venció
    public string Estado { get; set; } = string.Empty;
    // Ej: "accredited" cuando el pago se acreditó
    public string DetalleEstado { get; set; } = string.Empty;
    // El Id de nuestro Pago, que mandamos al crear la orden
    public string ReferenciaExterna { get; set; } = string.Empty;
    public string MedioPago { get; set; } = string.Empty;
}
