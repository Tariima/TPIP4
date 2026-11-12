using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class SolicitudCuentaGimnasioService : ISolicitudCuentaGimnasioService
{
    private readonly ISolicitudCuentaGimnasioRepository _solicitudCuentaGimnasioRepository;
    private readonly IGimnasioRepository _gimnasioRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public SolicitudCuentaGimnasioService(ISolicitudCuentaGimnasioRepository solicitudCuentaGimnasioRepository,
        IGimnasioRepository gimnasioRepository, IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _solicitudCuentaGimnasioRepository = solicitudCuentaGimnasioRepository;
        _gimnasioRepository = gimnasioRepository;
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<SolicitudCuentaGimnasioDTO> Create(CreationSolicitudCuentaGimnasioDTO creationSolicitudDto)
    {
        var gimnasio = await _gimnasioRepository.GetByIdAsync(creationSolicitudDto.GimnasioId);

        if (gimnasio is null || !gimnasio.Activo)
        {
            throw new NotFoundException($"No se encontró el gimnasio con id {creationSolicitudDto.GimnasioId}");
        }

        var usuarioExistente = await _usuarioRepository.GetByEmailAsync(creationSolicitudDto.Email);

        if (usuarioExistente is not null)
        {
            throw new ConflictException("Ya existe un usuario con ese email");
        }

        if (creationSolicitudDto.Contrasena.Length < 8 || creationSolicitudDto.Contrasena.Length > 20)
        {
            throw new ValidationException("La contraseña tiene que tener entre 8 y 20 caracteres");
        }

        // La cuenta se crea ya pero inactiva, no puede loguearse hasta que la aprueben.
        // La contraseña queda en la cuenta porque la solicitud no la guarda
        var nuevoAdmin = new AdminGimnasio();

        nuevoAdmin.Nombre = creationSolicitudDto.Nombre;
        nuevoAdmin.Apellido = creationSolicitudDto.Apellido;
        nuevoAdmin.Email = creationSolicitudDto.Email;
        nuevoAdmin.Contrasena = _passwordHasher.Hash(creationSolicitudDto.Contrasena);
        nuevoAdmin.FechaRegistro = DateTime.Now;
        nuevoAdmin.Activo = false;
        nuevoAdmin.GimnasioId = gimnasio.Id;

        await _usuarioRepository.CreateAsync(nuevoAdmin);

        var nuevaSolicitud = new SolicitudCuentaGimnasio();

        nuevaSolicitud.NombreSolicitante = $"{creationSolicitudDto.Nombre} {creationSolicitudDto.Apellido}";
        nuevaSolicitud.EmailSolicitante = creationSolicitudDto.Email;
        nuevaSolicitud.FechaSolicitud = DateTime.Now;
        nuevaSolicitud.Estado = EstadoSolicitud.Pendiente;
        nuevaSolicitud.GimnasioId = gimnasio.Id;
        nuevaSolicitud.Gimnasio = gimnasio;

        var solicitudCreada = await _solicitudCuentaGimnasioRepository.CreateAsync(nuevaSolicitud);
        return SolicitudCuentaGimnasioDTO.Create(solicitudCreada);
    }

    // sin estado trae todas
    public async Task<List<SolicitudCuentaGimnasioDTO>> GetAll(string? estado)
    {
        EstadoSolicitud? estadoFiltro = null;

        if (!string.IsNullOrWhiteSpace(estado))
        {
            if (!Enum.TryParse(estado.Trim(), true, out EstadoSolicitud estadoElegido) || !Enum.IsDefined(estadoElegido))
            {
                throw new ValidationException("El estado tiene que ser Pendiente, Aprobada o Rechazada");
            }

            estadoFiltro = estadoElegido;
        }

        var solicitudList = await _solicitudCuentaGimnasioRepository.GetConGimnasioAsync(estadoFiltro);
        return SolicitudCuentaGimnasioDTO.CreateList(solicitudList);
    }

    public async Task<SolicitudCuentaGimnasioDTO> GetById(int idSolicitud)
    {
        var solicitud = await _solicitudCuentaGimnasioRepository.GetByIdConGimnasioAsync(idSolicitud);

        if (solicitud is null)
        {
            throw new NotFoundException($"No se encontró la solicitud con id {idSolicitud}");
        }

        return SolicitudCuentaGimnasioDTO.Create(solicitud);
    }

    public async Task Aprobar(int idSuperAdmin, int idSolicitud)
    {
        var solicitud = await GetSolicitudPendiente(idSolicitud);

        if (!solicitud.Gimnasio.Activo)
        {
            throw new ConflictException("No se puede aprobar porque el gimnasio está dado de baja");
        }

        // la cuenta se busca por el email de la solicitud
        var admin = await _usuarioRepository.GetByEmailAsync(solicitud.EmailSolicitante) as AdminGimnasio;

        if (admin is null)
        {
            throw new NotFoundException("No se encontró la cuenta del solicitante");
        }

        admin.Activo = true;
        admin.FechaAprobacion = DateTime.Now;

        solicitud.Estado = EstadoSolicitud.Aprobada;
        solicitud.FechaResolucion = DateTime.Now;
        solicitud.SuperAdminId = idSuperAdmin;

        await _usuarioRepository.UpdateAsync(admin);
        await _solicitudCuentaGimnasioRepository.UpdateAsync(solicitud);

        await _emailService.EnviarAsync(
            admin.Email,
            "Tu cuenta de gimnasio fue aprobada",
            $"Hola {admin.Nombre}, aprobamos tu cuenta como admin de {solicitud.Gimnasio.Nombre}. Ya podés iniciar sesión.");
    }

    // la cuenta queda inactiva, así que no puede loguearse
    public async Task Rechazar(int idSuperAdmin, int idSolicitud)
    {
        var solicitud = await GetSolicitudPendiente(idSolicitud);

        solicitud.Estado = EstadoSolicitud.Rechazada;
        solicitud.FechaResolucion = DateTime.Now;
        solicitud.SuperAdminId = idSuperAdmin;

        await _solicitudCuentaGimnasioRepository.UpdateAsync(solicitud);
    }

    // solo se resuelven las pendientes
    private async Task<SolicitudCuentaGimnasio> GetSolicitudPendiente(int idSolicitud)
    {
        var solicitud = await _solicitudCuentaGimnasioRepository.GetByIdConGimnasioAsync(idSolicitud);

        if (solicitud is null)
        {
            throw new NotFoundException($"No se encontró la solicitud con id {idSolicitud}");
        }

        if (solicitud.Estado != EstadoSolicitud.Pendiente)
        {
            throw new ConflictException($"La solicitud ya fue {solicitud.Estado.ToString().ToLower()}");
        }

        return solicitud;
    }
}
