using Application.Models;

namespace Application.Interfaces;

public interface IProgresoService
{
    Task<List<ProgresoEjercicioDTO>> GetProgresoEjercicio(int idUsuario, int idEjercicio, DateTime? desde, DateTime? hasta);
    Task<List<ResumenSemanalDTO>> GetResumenSemanal(int idUsuario, DateTime? desde, DateTime? hasta);
}
