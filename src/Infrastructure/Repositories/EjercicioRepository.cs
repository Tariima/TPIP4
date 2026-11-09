using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EjercicioRepository : RepositoryBase<Ejercicio>, IEjercicioRepository
{
    private readonly ApplicationDbContext _context;

    public EjercicioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Ejercicio>> GetByGrupoMuscularAsync(string grupoMuscular)
    {
        return await _context.Ejercicios.Where(e => e.GrupoMuscular == grupoMuscular)
            .ToListAsync();
    }
}
