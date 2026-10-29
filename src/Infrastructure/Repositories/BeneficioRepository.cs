using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class BeneficioRepository : RepositoryBase<Beneficio>, IBeneficioRepository
{
    private readonly ApplicationDbContext _context;

    public BeneficioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
