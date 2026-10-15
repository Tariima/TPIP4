namespace Domain.Entities;

public class Beneficio
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public float PorcentajeDescuento { get; set; }
    public DateTime VigenteDesde { get; set; }
    public DateTime VigenteHasta { get; set; }
    public bool Activo { get; set; }

    public int GimnasioId { get; set; }
    public Gimnasio Gimnasio { get; set; }
}
