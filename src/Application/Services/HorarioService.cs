using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class HorarioService : IHorarioService
{
    private readonly IHorarioRepository _horarioRepository;

    public HorarioService(IHorarioRepository horarioRepository)
    {
        _horarioRepository = horarioRepository;
    }
}
