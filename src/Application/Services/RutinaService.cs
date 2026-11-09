using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class RutinaService : IRutinaService
{
    private readonly IRutinaRepository _rutinaRepository;
    private readonly IEjercicioRepository _ejercicioRepository;

    public RutinaService(IRutinaRepository rutinaRepository, IEjercicioRepository ejercicioRepository)
    {
        _rutinaRepository = rutinaRepository;
        _ejercicioRepository = ejercicioRepository;
    }

    public async Task<List<RutinaDTO>> GetAll(int idUsuario)
    {
        var rutinaList = await _rutinaRepository.GetActivasByUsuarioAsync(idUsuario);
        return RutinaDTO.CreateList(rutinaList);
    }

    public async Task<RutinaDTO> Create(int idUsuario, CreationRutinaDTO creationRutinaDto)
    {
        if (!creationRutinaDto.Ejercicios.Any())
        {
            throw new ValidationException("La rutina tiene que tener al menos un ejercicio");
        }

        var nuevaRutina = new Rutina();

        nuevaRutina.Nombre = creationRutinaDto.Nombre;
        nuevaRutina.Descripcion = creationRutinaDto.Descripcion;
        nuevaRutina.FechaCreacion = DateTime.Now;
        nuevaRutina.Activa = true;
        nuevaRutina.UsuarioComunId = idUsuario;

        foreach (var e in creationRutinaDto.Ejercicios)
        {
            if (e.SeriesObjetivo <= 0 || e.RepeticionesObjetivo <= 0)
            {
                throw new ValidationException("Las series y repeticiones objetivo deben ser mayores a cero");
            }

            var ejercicio = await _ejercicioRepository.GetByIdAsync(e.EjercicioId);

            if (ejercicio is null)
            {
                throw new NotFoundException($"No se encontró el ejercicio con id {e.EjercicioId}");
            }

            var ejercicioRutina = new EjercicioRutina();
            ejercicioRutina.Ejercicio = ejercicio;
            ejercicioRutina.Orden = e.Orden;
            ejercicioRutina.SeriesObjetivo = e.SeriesObjetivo;
            ejercicioRutina.RepeticionesObjetivo = e.RepeticionesObjetivo;

            nuevaRutina.EjerciciosRutina.Add(ejercicioRutina);
        }

        var rutinaCreada = await _rutinaRepository.CreateAsync(nuevaRutina);
        return RutinaDTO.Create(rutinaCreada);
    }

    public async Task<RutinaDTO> GetById(int idUsuario, int idRutina)
    {
        var rutina = await GetRutinaDelUsuario(idUsuario, idRutina);
        return RutinaDTO.Create(rutina);
    }

    // Devuelve la rutina solo si existe, está activa y es del usuario.
    // Si es de otro usuario también da 404, así no se sabe que existe
    private async Task<Rutina> GetRutinaDelUsuario(int idUsuario, int idRutina)
    {
        var rutina = await _rutinaRepository.GetByIdConEjerciciosAsync(idRutina);

        if (rutina is null || !rutina.Activa || rutina.UsuarioComunId != idUsuario)
        {
            throw new NotFoundException($"No se encontró la rutina con id {idRutina}");
        }

        return rutina;
    }
}