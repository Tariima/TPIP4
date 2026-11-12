using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BeneficioRepository : RepositoryBase<Beneficio>, IBeneficioRepository
{
    private readonly ApplicationDbContext _context;

    public BeneficioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Beneficio>> GetActivosByGimnasioAsync(int gimnasioId)
    {
        return await _context.Beneficios
            .Where(b => b.GimnasioId == gimnasioId && b.Activo)
            .ToListAsync();
    }

    // Vigentes en esa fecha y de gimnasios activos. Con .Date entran completos el primer y el último día
    public async Task<List<Beneficio>> GetVigentesAsync(DateTime fecha)
    {
        return await _context.Beneficios
            .Include(b => b.Gimnasio)
            .Where(b => b.Activo
                && b.Gimnasio.Activo
                && b.VigenteDesde.Date <= fecha.Date
                && b.VigenteHasta.Date >= fecha.Date)
            .ToListAsync();
    }
}
