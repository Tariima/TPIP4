using Application.Models;

namespace Application.Interfaces;

public interface IEjercicioService
{
    Task<List<EjercicioDTO>> GetAll(string? grupoMuscular);
    Task<EjercicioDTO> GetById(int id);
}
