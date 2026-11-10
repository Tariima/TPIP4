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
        ValidarDatosEntrenamiento(creationEntrenamientoDto.Fecha, creationEntrenamientoDto.DuracionMinutos);
        var rutina = await GetRutinaDelUsuario(idUsuario, creationEntrenamientoDto.RutinaId);
        var registrosSerie = await ArmarRegistrosSerie(creationEntrenamientoDto.Series);

        var nuevoEntrenamiento = new Entrenamiento();

        nuevoEntrenamiento.Fecha = creationEntrenamientoDto.Fecha;
        nuevoEntrenamiento.DuracionMinutos = creationEntrenamientoDto.DuracionMinutos;
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
        ValidarDatosEntrenamiento(updateEntrenamientoDto.Fecha, updateEntrenamientoDto.DuracionMinutos);

        // La rutina se valida solo si cambió. Si el entrenamiento se basó en una rutina que
        // después se dio de baja, se tiene que poder seguir editando sin que dé 404
        if (updateEntrenamientoDto.RutinaId != entrenamiento.RutinaId)
        {
            var rutina = await GetRutinaDelUsuario(idUsuario, updateEntrenamientoDto.RutinaId);
            entrenamiento.Rutina = rutina;
            entrenamiento.RutinaId = rutina?.Id;
        }

        var registrosSerie = await ArmarRegistrosSerie(updateEntrenamientoDto.Series);

        entrenamiento.Fecha = updateEntrenamientoDto.Fecha;
        entrenamiento.DuracionMinutos = updateEntrenamientoDto.DuracionMinutos;
        entrenamiento.Observaciones = updateEntrenamientoDto.Observaciones;

        // Las series anteriores se sacan de la lista y EF las borra de la tabla
        entrenamiento.RegistrosSerie.Clear();
        entrenamiento.RegistrosSerie.AddRange(registrosSerie);

        await _entrenamientoRepository.UpdateAsync(entrenamiento);
    }

    // Borrado físico: del entrenamiento no depende nada y sus series se borran en cascada
    public async Task Delete(int idUsuario, int idEntrenamiento)
    {
        var entrenamiento = await GetEntrenamientoDelUsuario(idUsuario, idEntrenamiento);

        await _entrenamientoRepository.DeleteAsync(entrenamiento);
    }

    // Devuelve el entrenamiento solo si existe y es del usuario.
    // Si es de otro usuario también da 404, así no se sabe que existe.
    // Lo van a usar también Update y Delete
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
        // No se puede registrar un entrenamiento que todavía no se hizo
        if (fecha > DateTime.Now)
        {
            throw new ValidationException("La fecha del entrenamiento no puede ser futura");
        }

        if (duracionMinutos <= 0)
        {
            throw new ValidationException("La duración tiene que ser mayor a cero");
        }
    }

    // Si no se mandó rutina devuelve null. Si se mandó, tiene que existir, estar activa y ser del usuario
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

    // Valida las series que mandó el cliente y arma las filas de RegistroSerie.
    // Lo va a usar también Update, así las reglas están en un solo lugar
    private async Task<List<RegistroSerie>> ArmarRegistrosSerie(List<CreationRegistroSerieDTO> series)
    {
        if (!series.Any())
        {
            throw new ValidationException("El entrenamiento tiene que tener al menos una serie");
        }

        var registroSerieList = new List<RegistroSerie>();

        foreach (var s in series)
        {
            // El peso puede ser 0 para ejercicios con el peso del cuerpo, como dominadas
            if (s.NumeroSerie <= 0 || s.Repeticiones <= 0 || s.Peso < 0)
            {
                throw new ValidationException("El número de serie y las repeticiones deben ser mayores a cero y el peso no puede ser negativo");
            }

            var ejercicio = await _ejercicioRepository.GetByIdAsync(s.EjercicioId);

            if (ejercicio is null)
            {
                throw new NotFoundException($"No se encontró el ejercicio con id {s.EjercicioId}");
            }

            var registroSerie = new RegistroSerie();
            registroSerie.Ejercicio = ejercicio;
            registroSerie.NumeroSerie = s.NumeroSerie;
            registroSerie.Peso = s.Peso;
            registroSerie.Repeticiones = s.Repeticiones;

            registroSerieList.Add(registroSerie);
        }

        return registroSerieList;
    }
}
