using Domain.Entities;

namespace Domain.Interfaces;

public interface IRutinaRepository : IRepositoryBase<Rutina>
{
    Task<List<Rutina>> GetActivasByUsuarioAsync(int usuarioComunId);
    Task<Rutina?> GetByIdConEjerciciosAsync(int id);
}