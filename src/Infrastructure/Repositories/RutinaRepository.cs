using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RutinaRepository : RepositoryBase<Rutina>, IRutinaRepository
{
    private readonly ApplicationDbContext _context;

    public RutinaRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Rutina>> GetActivasByUsuarioAsync(int usuarioComunId)
    {
        return await _context.Rutinas
            .Include(r => r.EjerciciosRutina)
                .ThenInclude(er => er.Ejercicio)
            .Where(r => r.UsuarioComunId == usuarioComunId && r.Activa)
            .ToListAsync();
    }

    public async Task<Rutina?> GetByIdConEjerciciosAsync(int id)
    {
        return await _context.Rutinas
            .Include(r => r.EjerciciosRutina)
                .ThenInclude(er => er.Ejercicio)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
