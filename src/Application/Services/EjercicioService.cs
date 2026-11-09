using Application.Interfaces;
using Application.Models;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class EjercicioService : IEjercicioService
{
    private readonly IEjercicioRepository _ejercicioRepository;

    public EjercicioService(IEjercicioRepository ejercicioRepository)
    {
        _ejercicioRepository = ejercicioRepository;
    }

    public async Task<List<EjercicioDTO>> GetAll(string? grupoMuscular)
    {
        if (string.IsNullOrWhiteSpace(grupoMuscular))
        {
            var ejercicioList = await _ejercicioRepository.GetAllAsync();
            return EjercicioDTO.CreateList(ejercicioList);
        }

        var filteredEjercicioList = await _ejercicioRepository.GetByGrupoMuscularAsync(grupoMuscular.Trim());

        if (!filteredEjercicioList.Any())
        {
            throw new NotFoundException($"No se encontraron ejercicios del grupo muscular: {grupoMuscular.Trim()}");
        }

        return EjercicioDTO.CreateList(filteredEjercicioList);
    }

    public async Task<EjercicioDTO> GetById(int id)
    {
        var ejercicio = await _ejercicioRepository.GetByIdAsync(id);

        if (ejercicio is null)
        {
            throw new NotFoundException($"No se encontró el ejercicio con id {id}");
        }

        return EjercicioDTO.Create(ejercicio);
    }
}
