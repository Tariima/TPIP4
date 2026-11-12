using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EntrenamientoRepository : RepositoryBase<Entrenamiento>, IEntrenamientoRepository
{
    private readonly ApplicationDbContext _context;

    public EntrenamientoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Entrenamiento>> GetByUsuarioAsync(int usuarioComunId, DateTime? desde, DateTime? hasta)
    {
        var query = _context.Entrenamientos
            .Include(e => e.Rutina)
            .Include(e => e.RegistrosSerie)
                .ThenInclude(rs => rs.Ejercicio)
            .Where(e => e.UsuarioComunId == usuarioComunId);

        // Filtros opcionales. Con .Date "hasta" incluye todo ese día
        if (desde is not null)
        {
            query = query.Where(e => e.Fecha.Date >= desde.Value.Date);
        }

        if (hasta is not null)
        {
            query = query.Where(e => e.Fecha.Date <= hasta.Value.Date);
        }

        return await query
            .OrderByDescending(e => e.Fecha)
            .ToListAsync();
    }

    public async Task<Entrenamiento?> GetByIdConSeriesAsync(int id)
    {
        return await _context.Entrenamientos
            .Include(e => e.Rutina)
            .Include(e => e.RegistrosSerie)
                .ThenInclude(rs => rs.Ejercicio)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
