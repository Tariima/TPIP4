using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class EntrenamientoService : IEntrenamientoService
{
    private readonly IEntrenamientoRepository _entrenamientoRepository;
    private readonly IRutinaRepository _rutinaRepository;
    private readonly IEjercicioRepository _ejercicioRepository;

    public EntrenamientoService(IEntrenamientoRepository entrenamientoRepository, IRutinaRepository rutinaRepository, IEjercicioRepository ejercicioRepository)
    {
        _entrenamientoRepository = entrenamientoRepository;
        _rutinaRepository = rutinaRepository;
        _ejercicioRepository = ejercicioRepository;
    }

    public async Task<List<EntrenamientoDTO>> GetAll(int idUsuario, DateTime? desde, DateTime? hasta)
    {
        if (desde is not null && hasta is not null && desde > hasta)
        {
            throw new ValidationException("La fecha desde no puede ser posterior a la fecha hasta");
        }

        var entrenamientoList = await _entrenamientoRepository.GetByUsuarioAsync(idUsuario, desde, hasta);
        return EntrenamientoDTO.CreateList(entrenamientoList);
    }

    public async Task<EntrenamientoDTO> GetById(int idUsuario, int idEntrenamiento)
    {
        var entrenamiento = await GetEntrenamientoDelUsuario(idUsuario, idEntrenamiento);
        return EntrenamientoDTO.Create(entrenamiento);
    }

    public async Task<EntrenamientoDTO> Create(int idUsuario, CreationEntrenamientoDTO creationEntrenamientoDto)
    {
        // .Value es seguro, el [Required] ya validó
        DateTime fecha = creationEntrenamientoDto.Fecha!.Value;
        int duracionMinutos = creationEntrenamientoDto.DuracionMinutos!.Value;

        ValidarDatosEntrenamiento(fecha, duracionMinutos);
        var rutina = await GetRutinaDelUsuario(idUsuario, creationEntrenamientoDto.RutinaId);
        var registrosSerie = await ArmarRegistrosSerie(creationEntrenamientoDto.Series);

        var nuevoEntrenamiento = new Entrenamiento();

        nuevoEntrenamiento.Fecha = fecha;
        nuevoEntrenamiento.DuracionMinutos = duracionMinutos;
        nuevoEntrenamiento.Observaciones = creationEntrenamientoDto.Observaciones;
        nuevoEntrenamiento.UsuarioComunId = idUsuario;
        nuevoEntrenamiento.Rutina = rutina;
        nuevoEntrenamiento.RegistrosSerie = registrosSerie;

        var entrenamientoCreado = await _entrenamientoRepository.CreateAsync(nuevoEntrenamiento);
        return EntrenamientoDTO.Create(entrenamientoCreado);
    }

    public async Task Update(int idUsuario, int idEntrenamiento, UpdateEntrenamientoDTO updateEntrenamientoDto)
    {
        var entrenamiento = await GetEntrenamientoDelUsuario(idUsuario, idEntrenamiento);

        DateTime fecha = updateEntrenamientoDto.Fecha!.Value;
        int duracionMinutos = updateEntrenamientoDto.DuracionMinutos!.Value;

        ValidarDatosEntrenamiento(fecha, duracionMinutos);

        // La rutina se valida solo si cambió: si la original se dio de baja, igual se tiene que poder editar
        if (updateEntrenamientoDto.RutinaId != entrenamiento.RutinaId)
        {
            var rutina = await GetRutinaDelUsuario(idUsuario, updateEntrenamientoDto.RutinaId);
            entrenamiento.Rutina = rutina;
            entrenamiento.RutinaId = rutina?.Id;
        }

        var registrosSerie = await ArmarRegistrosSerie(updateEntrenamientoDto.Series);

        entrenamiento.Fecha = fecha;
        entrenamiento.DuracionMinutos = duracionMinutos;
        entrenamiento.Observaciones = updateEntrenamientoDto.Observaciones;

        // EF borra las series que salen de la lista
        entrenamiento.RegistrosSerie.Clear();
        entrenamiento.RegistrosSerie.AddRange(registrosSerie);

        await _entrenamientoRepository.UpdateAsync(entrenamiento);
    }

    // Borrado físico, las series se van en cascada
    public async Task Delete(int idUsuario, int idEntrenamiento)
    {
        var entrenamiento = await GetEntrenamientoDelUsuario(idUsuario, idEntrenamiento);

        await _entrenamientoRepository.DeleteAsync(entrenamiento);
    }

    // 404 si no existe o es de otro usuario
    private async Task<Entrenamiento> GetEntrenamientoDelUsuario(int idUsuario, int idEntrenamiento)
    {
        var entrenamiento = await _entrenamientoRepository.GetByIdConSeriesAsync(idEntrenamiento);

        if (entrenamiento is null || entrenamiento.UsuarioComunId != idUsuario)
        {
            throw new NotFoundException($"No se encontró el entrenamiento con id {idEntrenamiento}");
        }

        return entrenamiento;
    }

    private void ValidarDatosEntrenamiento(DateTime fecha, int duracionMinutos)
    {
        // no se puede cargar algo que todavía no pasó
        if (fecha > DateTime.Now)
        {
            throw new ValidationException("La fecha del entrenamiento no puede ser futura");
        }

        if (duracionMinutos <= 0)
        {
            throw new ValidationException("La duración tiene que ser mayor a cero");
        }
    }

    // null si no mandó rutina
    private async Task<Rutina?> GetRutinaDelUsuario(int idUsuario, int? idRutina)
    {
        if (idRutina is null)
        {
            return null;
        }

        var rutina = await _rutinaRepository.GetByIdAsync(idRutina.Value);

        if (rutina is null || !rutina.Activa || rutina.UsuarioComunId != idUsuario)
        {
            throw new NotFoundException($"No se encontró la rutina con id {idRutina}");
        }

        return rutina;
    }

    // Valida las series y arma los RegistroSerie (lo usan Create y Update)
    private async Task<List<RegistroSerie>> ArmarRegistrosSerie(List<CreationRegistroSerieDTO> series)
    {
        if (!series.Any())
        {
            throw new ValidationException("El entrenamiento tiene que tener al menos una serie");
        }

        var registroSerieList = new List<RegistroSerie>();

        foreach (var s in series)
        {
            // el [Required] valida cada serie de la lista, acá ya vienen completas
            int ejercicioId = s.EjercicioId!.Value;
            int numeroSerie = s.NumeroSerie!.Value;
            float peso = s.Peso!.Value;
            int repeticiones = s.Repeticiones!.Value;

            // peso 0 = peso corporal (dominadas, etc.)
            if (numeroSerie <= 0 || repeticiones <= 0 || peso < 0)
            {
                throw new ValidationException("El número de serie y las repeticiones deben ser mayores a cero y el peso no puede ser negativo");
            }

            var ejercicio = await _ejercicioRepository.GetByIdAsync(ejercicioId);

            if (ejercicio is null)
            {
                throw new NotFoundException($"No se encontró el ejercicio con id {ejercicioId}");
            }

            var registroSerie = new RegistroSerie();
            registroSerie.Ejercicio = ejercicio;
            registroSerie.NumeroSerie = numeroSerie;
            registroSerie.Peso = peso;
            registroSerie.Repeticiones = repeticiones;

            registroSerieList.Add(registroSerie);
        }

        return registroSerieList;
    }
}
