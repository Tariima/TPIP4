namespace Domain.Entities;

public class AdminGimnasio : Usuario
{
    public DateTime? FechaAprobacion { get; set; }

    public int GimnasioId { get; set; }
    public Gimnasio Gimnasio { get; set; }
}
