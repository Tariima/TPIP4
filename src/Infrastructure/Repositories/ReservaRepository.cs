using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReservaRepository : RepositoryBase<Reserva>, IReservaRepository
{
    private readonly ApplicationDbContext _context;

    public ReservaRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    // lugares ocupados en ese horario y fecha (sin las canceladas)
    public async Task<int> CountConfirmadasAsync(int horarioId, DateTime fecha)
    {
        return await _context.Reservas
            .CountAsync(r => r.HorarioId == horarioId
                && r.FechaTurno.Date == fecha.Date
                && r.Estado == EstadoReserva.Confirmada);
    }

    public async Task<bool> TieneReservaConfirmadaAsync(int usuarioComunId, DateTime fecha)
    {
        return await _context.Reservas
            .AnyAsync(r => r.UsuarioComunId == usuarioComunId
                && r.FechaTurno.Date == fecha.Date
                && r.Estado == EstadoReserva.Confirmada);
    }

    public async Task<List<Reserva>> GetByUsuarioAsync(int usuarioComunId)
    {
        return await _context.Reservas
            .Include(r => r.Horario)
                .ThenInclude(h => h.Gimnasio)
            .Where(r => r.UsuarioComunId == usuarioComunId)
            .OrderByDescending(r => r.FechaTurno)
            .ToListAsync();
    }

    public async Task<Reserva?> GetByIdConHorarioAsync(int id)
    {
        return await _context.Reservas
            .Include(r => r.Horario)
                .ThenInclude(h => h.Gimnasio)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    // reservas del gimnasio entre las dos fechas, inclusive
    public async Task<List<Reserva>> GetByGimnasioAsync(int gimnasioId, DateTime desde, DateTime hasta)
    {
        return await _context.Reservas
            .Include(r => r.Horario)
            .Include(r => r.UsuarioComun)
            .Where(r => r.Horario.GimnasioId == gimnasioId
                && r.FechaTurno.Date >= desde.Date
                && r.FechaTurno.Date <= hasta.Date)
            .OrderBy(r => r.FechaTurno)
            .ToListAsync();
    }
}
