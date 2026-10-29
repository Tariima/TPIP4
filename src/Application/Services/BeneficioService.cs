using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class BeneficioService : IBeneficioService
{
    private readonly IBeneficioRepository _beneficioRepository;

    public BeneficioService(IBeneficioRepository beneficioRepository)
    {
        _beneficioRepository = beneficioRepository;
    }
}
