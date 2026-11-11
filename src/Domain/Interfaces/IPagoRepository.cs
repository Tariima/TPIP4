using Domain.Entities;

namespace Domain.Interfaces;

public interface IPagoRepository : IRepositoryBase<Pago>
{
    Task<Pago?> GetByIdConSuscripcionAsync(int id);
}
