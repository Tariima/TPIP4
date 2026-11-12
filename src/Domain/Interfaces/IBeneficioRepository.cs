using Domain.Entities;

namespace Domain.Interfaces;

public interface IBeneficioRepository : IRepositoryBase<Beneficio>
{
    Task<List<Beneficio>> GetActivosByGimnasioAsync(int gimnasioId);
    Task<List<Beneficio>> GetVigentesAsync(DateTime fecha);
}
