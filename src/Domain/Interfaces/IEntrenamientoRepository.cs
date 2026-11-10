using Domain.Entities;

namespace Domain.Interfaces;

public interface IEntrenamientoRepository : IRepositoryBase<Entrenamiento>
{
    Task<List<Entrenamiento>> GetByUsuarioAsync(int usuarioComunId, DateTime? desde, DateTime? hasta);
    Task<Entrenamiento?> GetByIdConSeriesAsync(int id);
}
