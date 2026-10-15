namespace Domain.Entities;

public abstract class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }
}
