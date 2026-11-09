using Domain.Entities;

namespace Domain.Interfaces;

public interface IUsuarioRepository : IRepositoryBase<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<UsuarioComun?> GetUsuarioComunByIdAsync(int id);
}
