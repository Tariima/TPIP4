namespace Domain.Entities;

public class Horario
{
    public int Id { get; set; }
    public string DiaSemana { get; set; } = string.Empty;
    public DateTime HoraInicio { get; set; }
    public DateTime HoraFin { get; set; }
    public int CupoMaximo { get; set; }
    public bool Activo { get; set; }

    public int GimnasioId { get; set; }
    public Gimnasio Gimnasio { get; set; }
    public List<Reserva> Reservas { get; set; } = new List<Reserva>();
}
