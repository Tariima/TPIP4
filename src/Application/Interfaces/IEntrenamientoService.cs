using Application.Models;
using Application.Models.Request;

namespace Application.Interfaces;

public interface IEntrenamientoService
{
    Task<List<EntrenamientoDTO>> GetAll(int idUsuario, DateTime? desde, DateTime? hasta);
    Task<EntrenamientoDTO> GetById(int idUsuario, int idEntrenamiento);
    Task<EntrenamientoDTO> Create(int idUsuario, CreationEntrenamientoDTO creationEntrenamientoDto);
    Task Update(int idUsuario, int idEntrenamiento, UpdateEntrenamientoDTO updateEntrenamientoDto);
}
