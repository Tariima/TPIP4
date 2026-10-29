using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class SuscripcionRepository : RepositoryBase<Suscripcion>, ISuscripcionRepository
{
    private readonly ApplicationDbContext _context;

    public SuscripcionRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
