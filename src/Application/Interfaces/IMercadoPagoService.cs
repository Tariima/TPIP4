using Application.Models;

namespace Application.Interfaces;

// La implementación está en Infrastructure (llama a la API de MP)
public interface IMercadoPagoService
{
    Task<OrdenMercadoPagoDTO> CrearOrdenAsync(int idPago, decimal monto, string descripcion);
    Task<OrdenMercadoPagoDTO?> GetOrdenAsync(string idOrden);
}
