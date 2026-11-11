using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SolicitudCuentaGimnasioRepository : RepositoryBase<SolicitudCuentaGimnasio>, ISolicitudCuentaGimnasioRepository
{
    private readonly ApplicationDbContext _context;

    public SolicitudCuentaGimnasioRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    // Si no se pasa un estado trae todas las solicitudes
    public async Task<List<SolicitudCuentaGimnasio>> GetConGimnasioAsync(EstadoSolicitud? estado)
    {
        return await _context.SolicitudesCuentaGimnasio
            .Include(s => s.Gimnasio)
            .Where(s => estado == null || s.Estado == estado)
            .OrderBy(s => s.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<SolicitudCuentaGimnasio?> GetByIdConGimnasioAsync(int id)
    {
        return await _context.SolicitudesCuentaGimnasio
            .Include(s => s.Gimnasio)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}
