using Application.Models;
using Application.Models.Request;

namespace Application.Interfaces;

public interface IRutinaService
{
    Task<List<RutinaDTO>> GetAll(int idUsuario);
    Task<RutinaDTO> GetById(int idUsuario, int idRutina);
    Task<RutinaDTO> Create(int idUsuario, CreationRutinaDTO creationRutinaDto);
    Task Update(int idUsuario, int idRutina, UpdateRutinaDTO updateRutinaDto);
}