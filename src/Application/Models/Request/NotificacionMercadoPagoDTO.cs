namespace Application.Models.Request;

// Lo que manda Mercado Pago al webhook. Ej:
// { "action": "order.processed", "type": "order", "data": { "id": "ORD01JYH1Z1YJN4HZ8J3Q0RB3YP6D" } }
// Solo usamos el tipo y el id de la orden; el resto lo consultamos nosotros
public class NotificacionMercadoPagoDTO
{
    public string Type { get; set; } = string.Empty;
    public DatosNotificacionDTO Data { get; set; } = new DatosNotificacionDTO();
}

public class DatosNotificacionDTO
{
    public string Id { get; set; } = string.Empty;
}
