using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class SolicitudCuentaGimnasioRepository : RepositoryBase<SolicitudCuentaGimnasio>, ISolicitudCuentaGimnasioRepository
{
    private readonly ApplicationDbContext _context;

    public SolicitudCuentaGimnasioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
