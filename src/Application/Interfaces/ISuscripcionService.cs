using Application.Models;

namespace Application.Interfaces;

public interface ISuscripcionService
{
    Task<SuscripcionDTO> GetMiSuscripcion(int idUsuario);
    Task<LinkPagoDTO> Pagar(int idUsuario);
    Task ProcesarVencimientos();
}
