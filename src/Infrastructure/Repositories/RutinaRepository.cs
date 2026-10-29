using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class RutinaRepository : RepositoryBase<Rutina>, IRutinaRepository
{
    private readonly ApplicationDbContext _context;

    public RutinaRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
