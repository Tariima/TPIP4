using Application.Models;
using Application.Models.Request;

namespace Application.Interfaces;

public interface IRutinaService
{
    Task<List<RutinaDTO>> GetAll(int idUsuario);
    Task<RutinaDTO> Create(int idUsuario, CreationRutinaDTO creationRutinaDto);
}