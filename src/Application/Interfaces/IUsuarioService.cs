using Application.Models;
using Application.Models.Request;

namespace Application.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioDTO> Create(CreationUsuarioDTO creationUsuarioDto);
    Task<UsuarioComunDTO> GetById(int id);
    Task Update(int id, UpdateUsuarioDTO updateUsuarioDto);
    Task UpdateContrasena(int id, UpdateContrasenaDTO updateContrasenaDto);
}
