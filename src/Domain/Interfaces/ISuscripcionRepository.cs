using Domain.Entities;

namespace Domain.Interfaces;

public interface ISuscripcionRepository : IRepositoryBase<Suscripcion>
{
    Task<bool> TieneSuscripcionActivaAsync(int usuarioComunId);
    Task<Suscripcion?> GetByUsuarioAsync(int usuarioComunId);
    Task<List<Suscripcion>> GetActivasQueVencenEntreAsync(DateTime desde, DateTime hasta);
}
