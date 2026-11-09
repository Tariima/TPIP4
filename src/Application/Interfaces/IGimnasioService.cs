using Application.Models;
using Application.Models.Request;

namespace Application.Interfaces;

public interface IGimnasioService
{
    Task<List<GimnasioDTO>> GetActivos();
    Task<List<GimnasioDTO>> GetAll();
    Task<GimnasioDTO> GetById(int idGimnasio);
    Task<GimnasioDTO> Create(int idSuperAdmin, CreationGimnasioDTO creationGimnasioDto);
    Task Update(int idGimnasio, CreationGimnasioDTO creationGimnasioDto);
    Task Delete(int idGimnasio);
}
