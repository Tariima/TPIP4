using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HorarioRepository : RepositoryBase<Horario>, IHorarioRepository
{
    private readonly ApplicationDbContext _context;

    public HorarioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Horario>> GetActivosByGimnasioAsync(int gimnasioId)
    {
        return await _context.Horarios
            .Where(h => h.GimnasioId == gimnasioId && h.Activo)
            .ToListAsync();
    }

    public async Task<Horario?> GetByIdConGimnasioAsync(int id)
    {
        return await _context.Horarios
            .Include(h => h.Gimnasio)
            .FirstOrDefaultAsync(h => h.Id == id);
    }
}
