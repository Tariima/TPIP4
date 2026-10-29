using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class PagoRepository : RepositoryBase<Pago>, IPagoRepository
{
    private readonly ApplicationDbContext _context;

    public PagoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
