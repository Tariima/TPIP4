using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class HorarioRepository : RepositoryBase<Horario>, IHorarioRepository
{
    private readonly ApplicationDbContext _context;

    public HorarioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
