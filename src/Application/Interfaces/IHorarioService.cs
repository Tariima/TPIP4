using Application.Models;
using Application.Models.Request;

namespace Application.Interfaces;

public interface IHorarioService
{
    Task<List<HorarioDTO>> GetAll(int idAdmin);
    Task<HorarioDTO> GetById(int idAdmin, int idHorario);
    Task<HorarioDTO> Create(int idAdmin, CreationHorarioDTO creationHorarioDto);
    Task Update(int idAdmin, int idHorario, CreationHorarioDTO creationHorarioDto);
    Task Delete(int idAdmin, int idHorario);
}
