using Application.Models;

namespace Application.Interfaces;

public interface IRutinaService
{
    Task<List<RutinaDTO>> GetAll(int idUsuario);
}