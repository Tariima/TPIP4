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

    // Un usuario es Pro si tiene una suscripción activa que todavía no venció.
    // Se mira también la fecha porque el proceso que las pasa a vencidas corre una vez por día
    public async Task<bool> TieneSuscripcionActivaAsync(int usuarioComunId)
    {
        return await _context.Suscripciones
            .AnyAsync(s => s.UsuarioComunId == usuarioComunId
                && s.Estado == EstadoSuscripcion.Activa
                && s.FechaVencimiento > DateTime.Now);
    }

    // Cada usuario tiene una sola suscripción, que se va renovando con pagos nuevos
    public async Task<Suscripcion?> GetByUsuarioAsync(int usuarioComunId)
    {
        return await _context.Suscripciones
            .Include(s => s.Pagos)
            .FirstOrDefaultAsync(s => s.UsuarioComunId == usuarioComunId);
    }

    // Suscripciones activas que vencen en el rango [desde, hasta).
    // Trae el usuario porque hace falta su email para avisarle
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
