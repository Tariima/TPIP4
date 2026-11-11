using Application.Models;

namespace Application.Interfaces;

public interface ISuscripcionService
{
    Task<LinkPagoDTO> Pagar(int idUsuario);
}
