using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PagoRepository : RepositoryBase<Pago>, IPagoRepository
{
    private readonly ApplicationDbContext _context;

    public PagoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Pago?> GetByIdConSuscripcionAsync(int id)
    {
        return await _context.Pagos
            .Include(p => p.Suscripcion)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
