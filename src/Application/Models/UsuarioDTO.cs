using Domain.Entities;

namespace Application.Models;

// No incluye la contraseña para que nunca salga en una respuesta
public class UsuarioDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }

    public static UsuarioDTO Create(Usuario usuario)
    {
        var dto = new UsuarioDTO();
        dto.Id = usuario.Id;
        dto.Nombre = usuario.Nombre;
        dto.Apellido = usuario.Apellido;
        dto.Email = usuario.Email;
        dto.FechaRegistro = usuario.FechaRegistro;
        dto.Activo = usuario.Activo;

        return dto;
    }
}
