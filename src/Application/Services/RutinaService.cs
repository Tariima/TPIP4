using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class RutinaService : IRutinaService
{
    private readonly IRutinaRepository _rutinaRepository;

    public RutinaService(IRutinaRepository rutinaRepository)
    {
        _rutinaRepository = rutinaRepository;
    }
}
