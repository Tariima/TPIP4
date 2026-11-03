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

    // Registro de un usuario común (los admins de gimnasio entran por la solicitud de cuenta)
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
}
