using Application.Models;
using Application.Models.Request;

namespace Application.Interfaces;

public interface IBeneficioService
{
    Task<List<BeneficioDTO>> GetAllDelGimnasio(int idAdmin);
    Task<BeneficioDTO> GetById(int idAdmin, int idBeneficio);
    Task<BeneficioDTO> Create(int idAdmin, CreationBeneficioDTO creationBeneficioDto);
    Task Update(int idAdmin, int idBeneficio, CreationBeneficioDTO creationBeneficioDto);
    Task Delete(int idAdmin, int idBeneficio);
    Task<List<GimnasioConBeneficiosDTO>> GetVigentes(int idUsuario);
}
