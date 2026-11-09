using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GimnasioRepository : RepositoryBase<Gimnasio>, IGimnasioRepository
{
    private readonly ApplicationDbContext _context;

    public GimnasioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Gimnasio>> GetActivosAsync()
    {
        return await _context.Gimnasios
            .Where(g => g.Activo)
            .ToListAsync();
    }

    public async Task<Gimnasio?> GetByIdConHorariosYBeneficiosAsync(int id)
    {
        return await _context.Gimnasios
            .Include(g => g.Horarios)
            .Include(g => g.Beneficios)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}
