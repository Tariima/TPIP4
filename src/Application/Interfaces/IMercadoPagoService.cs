using Application.Models;

namespace Application.Interfaces;

// Servicio externo de pagos. La implementación está en Infrastructure porque habla por HTTP con Mercado Pago
public interface IMercadoPagoService
{
    Task<OrdenMercadoPagoDTO> CrearOrdenAsync(int idPago, decimal monto, string descripcion);
    Task<OrdenMercadoPagoDTO?> GetOrdenAsync(string idOrden);
}
