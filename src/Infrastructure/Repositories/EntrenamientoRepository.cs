using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class EntrenamientoRepository : RepositoryBase<Entrenamiento>, IEntrenamientoRepository
{
    private readonly ApplicationDbContext _context;

    public EntrenamientoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
