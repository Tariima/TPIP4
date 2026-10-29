using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class PagoService : IPagoService
{
    private readonly IPagoRepository _pagoRepository;

    public PagoService(IPagoRepository pagoRepository)
    {
        _pagoRepository = pagoRepository;
    }
}
