using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SuscripcionRepository : RepositoryBase<Suscripcion>, ISuscripcionRepository
{
    private readonly ApplicationDbContext _context;

    public SuscripcionRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    // Pro = activa y sin vencer. Se mira la fecha también porque el job que las vence corre una vez por día
    public async Task<bool> TieneSuscripcionActivaAsync(int usuarioComunId)
    {
        return await _context.Suscripciones
            .AnyAsync(s => s.UsuarioComunId == usuarioComunId
                && s.Estado == EstadoSuscripcion.Activa
                && s.FechaVencimiento > DateTime.Now);
    }

    // una suscripción por usuario, se renueva con pagos nuevos
    public async Task<Suscripcion?> GetByUsuarioAsync(int usuarioComunId)
    {
        return await _context.Suscripciones
            .Include(s => s.Pagos)
            .FirstOrDefaultAsync(s => s.UsuarioComunId == usuarioComunId);
    }

    // Activas que vencen en [desde, hasta). Trae el usuario para el mail
    public async Task<List<Suscripcion>> GetActivasQueVencenEntreAsync(DateTime desde, DateTime hasta)
    {
        return await _context.Suscripciones
            .Include(s => s.UsuarioComun)
            .Where(s => s.Estado == EstadoSuscripcion.Activa
                && s.FechaVencimiento >= desde
                && s.FechaVencimiento < hasta)
            .ToListAsync();
    }
}
