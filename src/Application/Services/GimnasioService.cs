using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class GimnasioService : IGimnasioService
{
    private readonly IGimnasioRepository _gimnasioRepository;

    public GimnasioService(IGimnasioRepository gimnasioRepository)
    {
        _gimnasioRepository = gimnasioRepository;
    }
}
