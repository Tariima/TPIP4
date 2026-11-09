using Application.Interfaces;
using Application.Models;
using Domain.Interfaces;

namespace Application.Services;

public class RutinaService : IRutinaService
{
    private readonly IRutinaRepository _rutinaRepository;

    public RutinaService(IRutinaRepository rutinaRepository)
    {
        _rutinaRepository = rutinaRepository;
    }

    public async Task<List<RutinaDTO>> GetAll(int idUsuario)
    {
        var rutinaList = await _rutinaRepository.GetActivasByUsuarioAsync(idUsuario);
        return RutinaDTO.CreateList(rutinaList);
    }
}