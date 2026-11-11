using Domain.Entities;

namespace Domain.Interfaces;

public interface IReservaRepository : IRepositoryBase<Reserva>
{
    Task<int> CountConfirmadasAsync(int horarioId, DateTime fecha);
    Task<bool> TieneReservaConfirmadaAsync(int usuarioComunId, DateTime fecha);
    Task<List<Reserva>> GetByUsuarioAsync(int usuarioComunId);
    Task<Reserva?> GetByIdConHorarioAsync(int id);
}
