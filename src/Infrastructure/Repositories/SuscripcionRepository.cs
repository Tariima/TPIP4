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
}
