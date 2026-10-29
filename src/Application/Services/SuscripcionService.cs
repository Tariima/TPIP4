using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class SuscripcionService : ISuscripcionService
{
    private readonly ISuscripcionRepository _suscripcionRepository;

    public SuscripcionService(ISuscripcionRepository suscripcionRepository)
    {
        _suscripcionRepository = suscripcionRepository;
    }
}
