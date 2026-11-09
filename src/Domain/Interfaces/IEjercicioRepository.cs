using Domain.Entities;

namespace Domain.Interfaces;

public interface IEjercicioRepository : IRepositoryBase<Ejercicio>
{
    Task<List<Ejercicio>> GetByGrupoMuscularAsync(string grupoMuscular);
}
