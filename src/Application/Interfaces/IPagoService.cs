using Application.Models.Request;

namespace Application.Interfaces;

public interface IPagoService
{
    Task ProcesarNotificacion(NotificacionMercadoPagoDTO notificacion);
}
