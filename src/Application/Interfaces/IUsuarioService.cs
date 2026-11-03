using Application.Models;
using Application.Models.Request;

namespace Application.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioDTO> Create(CreationUsuarioDTO creationUsuarioDto);
}
