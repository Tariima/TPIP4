using Domain.Entities;

namespace Domain.Interfaces;

public interface IHorarioRepository : IRepositoryBase<Horario>
{
    Task<List<Horario>> GetActivosByGimnasioAsync(int gimnasioId);
    Task<Horario?> GetByIdConGimnasioAsync(int id);
}
