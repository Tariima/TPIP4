using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class EntrenamientoService : IEntrenamientoService
{
    private readonly IEntrenamientoRepository _entrenamientoRepository;

    public EntrenamientoService(IEntrenamientoRepository entrenamientoRepository)
    {
        _entrenamientoRepository = entrenamientoRepository;
    }
}
