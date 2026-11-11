using Application.Models;
using Application.Models.Request;

namespace Application.Interfaces;

public interface IReservaService
{
    Task<List<HorarioDisponibleDTO>> GetHorariosDisponibles(int gimnasioId, DateTime fecha);
    Task<List<ReservaDTO>> GetMisReservas(int idUsuario);
    Task<ReservaDTO> GetById(int idUsuario, int idReserva);
    Task<ReservaDTO> Create(int idUsuario, CreationReservaDTO creationReservaDto);
    Task Cancelar(int idUsuario, int idReserva);
}
