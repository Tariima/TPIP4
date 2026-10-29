using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class GimnasioRepository : RepositoryBase<Gimnasio>, IGimnasioRepository
{
    private readonly ApplicationDbContext _context;

    public GimnasioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
