using Domain.Entities;

namespace Domain.Interfaces;

public interface IGimnasioRepository : IRepositoryBase<Gimnasio>
{
    Task<List<Gimnasio>> GetActivosAsync();
    Task<Gimnasio?> GetByIdConHorariosYBeneficiosAsync(int id);
}
