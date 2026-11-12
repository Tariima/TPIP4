using Domain.Entities;

namespace Application.Models;

// sin la contraseña
public class UsuarioComunDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public float Peso { get; set; }
    public float Altura { get; set; }
    public int PuntajeTotal { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }

    public static UsuarioComunDTO Create(UsuarioComun usuario)
    {
        var dto = new UsuarioComunDTO();
        dto.Id = usuario.Id;
        dto.Nombre = usuario.Nombre;
        dto.Apellido = usuario.Apellido;
        dto.Email = usuario.Email;
        dto.FechaNacimiento = usuario.FechaNacimiento;
        dto.Peso = usuario.Peso;
        dto.Altura = usuario.Altura;
        dto.PuntajeTotal = usuario.PuntajeTotal;
        dto.FechaRegistro = usuario.FechaRegistro;
        dto.Activo = usuario.Activo;

        return dto;
    }
}
