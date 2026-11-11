using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System.Globalization;

namespace Application.Services;

public class HorarioService : IHorarioService
{
    // Días válidos para un horario. Se guardan siempre escritos así, sin importar cómo los mande el admin
    private static readonly string[] DiasSemana = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

    private readonly IHorarioRepository _horarioRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IGimnasioRepository _gimnasioRepository;

    public HorarioService(IHorarioRepository horarioRepository, IUsuarioRepository usuarioRepository, IGimnasioRepository gimnasioRepository)
    {
        _horarioRepository = horarioRepository;
        _usuarioRepository = usuarioRepository;
        _gimnasioRepository = gimnasioRepository;
    }

    public async Task<List<HorarioDTO>> GetAll(int idAdmin)
    {
        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);
        var horarioList = await _horarioRepository.GetActivosByGimnasioAsync(gimnasioId);

        // Se ordenan por día de la semana (empezando por el lunes) y después por hora
        var horariosOrdenados = horarioList
            .OrderBy(h => Array.IndexOf(DiasSemana, h.DiaSemana))
            .ThenBy(h => h.HoraInicio)
            .ToList();

        return HorarioDTO.CreateList(horariosOrdenados);
    }

    public async Task<HorarioDTO> GetById(int idAdmin, int idHorario)
    {
        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);
        var horario = await GetHorarioDelGimnasio(gimnasioId, idHorario);

        return HorarioDTO.Create(horario);
    }

    public async Task<HorarioDTO> Create(int idAdmin, CreationHorarioDTO creationHorarioDto)
    {
        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);

        var nuevoHorario = new Horario();

        CargarDatos(nuevoHorario, creationHorarioDto);
        nuevoHorario.Activo = true;
        nuevoHorario.GimnasioId = gimnasioId;

        var horarioCreado = await _horarioRepository.CreateAsync(nuevoHorario);
        return HorarioDTO.Create(horarioCreado);
    }

    public async Task Update(int idAdmin, int idHorario, CreationHorarioDTO creationHorarioDto)
    {
        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);
        var horario = await GetHorarioDelGimnasio(gimnasioId, idHorario);

        CargarDatos(horario, creationHorarioDto);

        await _horarioRepository.UpdateAsync(horario);
    }

    // Se desactiva en vez de borrarlo, así no se pierden las reservas que se hicieron en ese horario
    public async Task Delete(int idAdmin, int idHorario)
    {
        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);
        var horario = await GetHorarioDelGimnasio(gimnasioId, idHorario);

        horario.Activo = false;

        await _horarioRepository.UpdateAsync(horario);
    }

    // Valida los datos que mandó el admin y los pasa al horario.
    // Lo usan Create y Update, así las reglas están en un solo lugar
    private void CargarDatos(Horario horario, CreationHorarioDTO creationHorarioDto)
    {
        // Se compara sin importar mayúsculas ni tildes: "miercoles" vale como "Miércoles"
        string? dia = DiasSemana.FirstOrDefault(d => string.Compare(d, creationHorarioDto.DiaSemana.Trim(),
            CultureInfo.InvariantCulture, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0);

        if (dia is null)
        {
            throw new ValidationException("El día de la semana no es válido, tiene que ser de Lunes a Domingo");
        }

        DateTime horaInicio = ConvertirHora(creationHorarioDto.HoraInicio, "inicio");
        DateTime horaFin = ConvertirHora(creationHorarioDto.HoraFin, "fin");

        if (horaInicio >= horaFin)
        {
            throw new ValidationException("La hora de inicio tiene que ser menor a la hora de fin");
        }

        if (creationHorarioDto.CupoMaximo <= 0)
        {
            throw new ValidationException("El cupo tiene que ser mayor a cero");
        }

        horario.DiaSemana = dia;
        horario.HoraInicio = horaInicio;
        horario.HoraFin = horaFin;
        horario.CupoMaximo = creationHorarioDto.CupoMaximo;
    }

    // Pasa una hora "HH:mm" a DateTime. Del horario solo importa la hora (el día va en DiaSemana),
    // por eso se guarda siempre con la misma fecha y así las horas se pueden comparar entre sí
    private DateTime ConvertirHora(string hora, string nombre)
    {
        if (!DateTime.TryParseExact(hora.Trim(), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime horaConvertida))
        {
            throw new ValidationException($"La hora de {nombre} no es válida, tiene que tener el formato HH:mm (ej: 08:30)");
        }

        return DateTime.MinValue.Add(horaConvertida.TimeOfDay);
    }

    // Cada admin maneja solo los horarios de su gimnasio. El gimnasio sale de su cuenta, no de lo que manda el cliente
    private async Task<int> GetGimnasioDelAdmin(int idAdmin)
    {
        var admin = await _usuarioRepository.GetByIdAsync(idAdmin) as AdminGimnasio;

        if (admin is null)
        {
            throw new NotFoundException("No se encontró la cuenta del admin");
        }

        var gimnasio = await _gimnasioRepository.GetByIdAsync(admin.GimnasioId);

        if (gimnasio is null || !gimnasio.Activo)
        {
            throw new ConflictException("El gimnasio está dado de baja");
        }

        return gimnasio.Id;
    }

    // Devuelve el horario solo si existe, está activo y es del gimnasio del admin.
    // Si es de otro gimnasio también da 404, así no se sabe que existe
    private async Task<Horario> GetHorarioDelGimnasio(int gimnasioId, int idHorario)
    {
        var horario = await _horarioRepository.GetByIdAsync(idHorario);

        if (horario is null || !horario.Activo || horario.GimnasioId != gimnasioId)
        {
            throw new NotFoundException($"No se encontró el horario con id {idHorario}");
        }

        return horario;
    }
}
