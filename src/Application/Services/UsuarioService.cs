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

        if (creationUsuarioDto.Peso <= 0 || creationUsuarioDto.Altura <= 0)
        {
            throw new ValidationException("El peso y la altura deben ser mayores a cero");
        }

        var nuevoUsuario = new UsuarioComun();

        nuevoUsuario.Nombre = creationUsuarioDto.Nombre;
        nuevoUsuario.Apellido = creationUsuarioDto.Apellido;
        nuevoUsuario.Email = creationUsuarioDto.Email;
        nuevoUsuario.Contrasena = _passwordHasher.Hash(creationUsuarioDto.Contrasena);
        nuevoUsuario.FechaNacimiento = creationUsuarioDto.FechaNacimiento;
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

        if (updateUsuarioDto.Peso <= 0 || updateUsuarioDto.Altura <= 0)
        {
            throw new ValidationException("El peso y la altura deben ser mayores a cero");
        }

        usuario.Nombre = updateUsuarioDto.Nombre;
        usuario.Apellido = updateUsuarioDto.Apellido;
        usuario.FechaNacimiento = updateUsuarioDto.FechaNacimiento;
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

        usuario.Contrasena = _passwordHasher.Hash(updateContrasenaDto.ContrasenaNueva);

        await _usuarioRepository.UpdateAsync(usuario);
    }
}
