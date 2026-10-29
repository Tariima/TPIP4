using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class EjercicioService : IEjercicioService
{
    private readonly IEjercicioRepository _ejercicioRepository;

    public EjercicioService(IEjercicioRepository ejercicioRepository)
    {
        _ejercicioRepository = ejercicioRepository;
    }
}
