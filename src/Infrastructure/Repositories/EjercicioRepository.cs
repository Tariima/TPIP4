using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class EjercicioRepository : RepositoryBase<Ejercicio>, IEjercicioRepository
{
    private readonly ApplicationDbContext _context;

    public EjercicioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
