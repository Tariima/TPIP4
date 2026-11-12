namespace Application.Models.Request;

// Body que manda MP al webhook, ej:
// { "action": "order.processed", "type": "order", "data": { "id": "ORD01JYH1Z1YJN4HZ8J3Q0RB3YP6D" } }
// Solo usamos type y data.id
public class NotificacionMercadoPagoDTO
{
    public string Type { get; set; } = string.Empty;
    public DatosNotificacionDTO Data { get; set; } = new DatosNotificacionDTO();
}

public class DatosNotificacionDTO
{
    public string Id { get; set; } = string.Empty;
}
