using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UsuarioService(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    // Solo usuarios comunes, los admins entran por la solicitud de cuenta
    public async Task<UsuarioDTO> Create(CreationUsuarioDTO creationUsuarioDto)
    {
        var usuarioExistente = await _usuarioRepository.GetByEmailAsync(creationUsuarioDto.Email);

        if (usuarioExistente is not null)
        {
            throw new ConflictException("Ya existe un usuario con ese email");
        }

        if (creationUsuarioDto.Contrasena.Length < 8 || creationUsuarioDto.Contrasena.Length > 20)
        {
            throw new ValidationException("La contraseña tiene que tener entre 8 y 20 caracteres");
        }

        // .Value es seguro, el [Required] ya validó
        DateTime fechaNacimiento = creationUsuarioDto.FechaNacimiento!.Value;

        ValidarDatosUsuario(fechaNacimiento, creationUsuarioDto.Peso, creationUsuarioDto.Altura);

        var nuevoUsuario = new UsuarioComun();

        nuevoUsuario.Nombre = creationUsuarioDto.Nombre;
        nuevoUsuario.Apellido = creationUsuarioDto.Apellido;
        nuevoUsuario.Email = creationUsuarioDto.Email;
        nuevoUsuario.Contrasena = _passwordHasher.Hash(creationUsuarioDto.Contrasena);
        nuevoUsuario.FechaNacimiento = fechaNacimiento;
        nuevoUsuario.Peso = creationUsuarioDto.Peso;
        nuevoUsuario.Altura = creationUsuarioDto.Altura;
        nuevoUsuario.FechaRegistro = DateTime.Now;
        nuevoUsuario.Activo = true;

        var usuarioAgregado = await _usuarioRepository.CreateAsync(nuevoUsuario);

        return UsuarioDTO.Create(usuarioAgregado);
    }

    public async Task<UsuarioComunDTO> GetById(int id)
    {
        var usuario = await _usuarioRepository.GetUsuarioComunByIdAsync(id);

        if (usuario is null)
        {
            throw new NotFoundException($"No se encontró el usuario con id {id}");
        }

        return UsuarioComunDTO.Create(usuario);
    }

    public async Task Update(int id, UpdateUsuarioDTO updateUsuarioDto)
    {
        var usuario = await _usuarioRepository.GetUsuarioComunByIdAsync(id);

        if (usuario is null)
        {
            throw new NotFoundException($"No se encontró el usuario con id {id}");
        }

        // .Value es seguro, el [Required] ya validó
        DateTime fechaNacimiento = updateUsuarioDto.FechaNacimiento!.Value;

        ValidarDatosUsuario(fechaNacimiento, updateUsuarioDto.Peso, updateUsuarioDto.Altura);

        usuario.Nombre = updateUsuarioDto.Nombre;
        usuario.Apellido = updateUsuarioDto.Apellido;
        usuario.FechaNacimiento = fechaNacimiento;
        usuario.Peso = updateUsuarioDto.Peso;
        usuario.Altura = updateUsuarioDto.Altura;

        await _usuarioRepository.UpdateAsync(usuario);
    }

    public async Task UpdateContrasena(int id, UpdateContrasenaDTO updateContrasenaDto)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);

        if (usuario is null)
        {
            throw new NotFoundException($"No se encontró el usuario con id {id}");
        }

        // se pide la actual para que no alcance con tener el token
        if (!_passwordHasher.Verify(updateContrasenaDto.ContrasenaActual, usuario.Contrasena))
        {
            throw new ValidationException("La contraseña actual es incorrecta");
        }

        if (updateContrasenaDto.ContrasenaNueva.Length < 8 || updateContrasenaDto.ContrasenaNueva.Length > 20)
        {
            throw new ValidationException("La contraseña nueva tiene que tener entre 8 y 20 caracteres");
        }

        usuario.Contrasena = _passwordHasher.Hash(updateContrasenaDto.ContrasenaNueva);

        await _usuarioRepository.UpdateAsync(usuario);
    }

    // Lo usan Create y Update, así las reglas están en un solo lugar
    private void ValidarDatosUsuario(DateTime fechaNacimiento, float peso, float altura)
    {
        if (fechaNacimiento.Date > DateTime.Today)
        {
            throw new ValidationException("La fecha de nacimiento no puede ser futura");
        }

        // Edad en años cumplidos: si este año todavía no cumplió, se resta uno
        int edad = DateTime.Today.Year - fechaNacimiento.Year;

        if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad))
        {
            edad--;
        }

        if (edad < 14 || edad > 100)
        {
            throw new ValidationException("La edad tiene que estar entre 14 y 100 años");
        }

        if (peso < 20 || peso > 300)
        {
            throw new ValidationException("El peso tiene que estar entre 20 y 300 kg");
        }

        // La altura va en metros (ej: 1.78)
        if (altura < 0.5 || altura > 2.5)
        {
            throw new ValidationException("La altura tiene que estar entre 0,5 y 2,5 metros");
        }
    }
}
