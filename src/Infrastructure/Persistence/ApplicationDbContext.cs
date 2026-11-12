using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<UsuarioComun> UsuariosComunes { get; set; }
    public DbSet<AdminGimnasio> AdminsGimnasio { get; set; }
    public DbSet<SuperAdmin> SuperAdmins { get; set; }
    public DbSet<Ejercicio> Ejercicios { get; set; }
    public DbSet<Rutina> Rutinas { get; set; }
    public DbSet<EjercicioRutina> EjerciciosRutina { get; set; }
    public DbSet<Entrenamiento> Entrenamientos { get; set; }
    public DbSet<RegistroSerie> RegistrosSerie { get; set; }
    public DbSet<Suscripcion> Suscripciones { get; set; }
    public DbSet<Pago> Pagos { get; set; }
    public DbSet<Gimnasio> Gimnasios { get; set; }
    public DbSet<SolicitudCuentaGimnasio> SolicitudesCuentaGimnasio { get; set; }
    public DbSet<Horario> Horarios { get; set; }
    public DbSet<Beneficio> Beneficios { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composiciones (ver diagrama): se borran en cascada
        modelBuilder.Entity<Rutina>()
            .HasOne(r => r.UsuarioComun)
            .WithMany(u => u.Rutinas)
            .HasForeignKey(r => r.UsuarioComunId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Entrenamiento>()
            .HasOne(e => e.UsuarioComun)
            .WithMany(u => u.Entrenamientos)
            .HasForeignKey(e => e.UsuarioComunId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EjercicioRutina>()
            .HasOne(er => er.Rutina)
            .WithMany(r => r.EjerciciosRutina)
            .HasForeignKey(er => er.RutinaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RegistroSerie>()
            .HasOne(rs => rs.Entrenamiento)
            .WithMany(e => e.RegistrosSerie)
            .HasForeignKey(rs => rs.EntrenamientoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Pago>()
            .HasOne(p => p.Suscripcion)
            .WithMany(s => s.Pagos)
            .HasForeignKey(p => p.SuscripcionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Horario>()
            .HasOne(h => h.Gimnasio)
            .WithMany(g => g.Horarios)
            .HasForeignKey(h => h.GimnasioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Beneficio>()
            .HasOne(b => b.Gimnasio)
            .WithMany(g => g.Beneficios)
            .HasForeignKey(b => b.GimnasioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Asociaciones: sin cascada
        modelBuilder.Entity<Entrenamiento>()
            .HasOne(e => e.Rutina)
            .WithMany(r => r.Entrenamientos)
            .HasForeignKey(e => e.RutinaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EjercicioRutina>()
            .HasOne(er => er.Ejercicio)
            .WithMany(e => e.EjerciciosRutina)
            .HasForeignKey(er => er.EjercicioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RegistroSerie>()
            .HasOne(rs => rs.Ejercicio)
            .WithMany(e => e.RegistrosSerie)
            .HasForeignKey(rs => rs.EjercicioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Suscripcion>()
            .HasOne(s => s.UsuarioComun)
            .WithMany(u => u.Suscripciones)
            .HasForeignKey(s => s.UsuarioComunId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.UsuarioComun)
            .WithMany(u => u.Reservas)
            .HasForeignKey(r => r.UsuarioComunId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Horario)
            .WithMany(h => h.Reservas)
            .HasForeignKey(r => r.HorarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Gimnasio>()
            .HasOne(g => g.SuperAdmin)
            .WithMany(s => s.Gimnasios)
            .HasForeignKey(g => g.SuperAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdminGimnasio>()
            .HasOne(a => a.Gimnasio)
            .WithMany(g => g.Admins)
            .HasForeignKey(a => a.GimnasioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitudCuentaGimnasio>()
            .HasOne(s => s.Gimnasio)
            .WithMany(g => g.Solicitudes)
            .HasForeignKey(s => s.GimnasioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitudCuentaGimnasio>()
            .HasOne(s => s.SuperAdmin)
            .WithMany(sa => sa.Solicitudes)
            .HasForeignKey(s => s.SuperAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Suscripcion>()
            .Property(s => s.MontoMensual)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Pago>()
            .Property(p => p.Monto)
            .HasPrecision(10, 2);

        // Catálogo de ejercicios
        modelBuilder.Entity<Ejercicio>().HasData(CreateEjercicioDataSeed());

        // El superadmin no se registra por la API, va por seed. Contraseña: "admin1234".
        // Va el hash ya calculado: si se genera acá cambia en cada ejecución y EF lo toma como un cambio en el seed
        modelBuilder.Entity<SuperAdmin>().HasData(new SuperAdmin
        {
            Id = 1,
            Nombre = "Super",
            Apellido = "Admin",
            Email = "superadmin@tpip4.com",
            Contrasena = "$2a$11$bjdAH2wJIDk04JQa3kP8cO9kgGk/conQEIKICUug66qzuzix0LINC",
            FechaRegistro = new DateTime(2026, 11, 9),
            Activo = true
        });

        base.OnModelCreating(modelBuilder);
    }

    private Ejercicio[] CreateEjercicioDataSeed()
    {
        Ejercicio[] ejercicios = [
            new Ejercicio
            {
                Id = 1,
                Nombre = "Press de banca",
                GrupoMuscular = "Pecho",
                Descripcion = "Acostado en el banco, bajar la barra hasta el pecho y empujarla hacia arriba."
            },
            new Ejercicio
            {
                Id = 2,
                Nombre = "Press inclinado con mancuernas",
                GrupoMuscular = "Pecho",
                Descripcion = "En el banco inclinado, empujar las mancuernas hacia arriba desde la altura del pecho."
            },
            new Ejercicio
            {
                Id = 3,
                Nombre = "Flexiones de brazos",
                GrupoMuscular = "Pecho",
                Descripcion = "Con el cuerpo recto, bajar el pecho hasta casi tocar el piso y volver a subir."
            },
            new Ejercicio
            {
                Id = 4,
                Nombre = "Dominadas",
                GrupoMuscular = "Espalda",
                Descripcion = "Colgado de la barra, subir el cuerpo hasta que el mentón pase la barra."
            },
            new Ejercicio
            {
                Id = 5,
                Nombre = "Remo con barra",
                GrupoMuscular = "Espalda",
                Descripcion = "Con el torso inclinado hacia adelante, llevar la barra hacia el abdomen."
            },
            new Ejercicio
            {
                Id = 6,
                Nombre = "Jalón al pecho",
                GrupoMuscular = "Espalda",
                Descripcion = "Sentado en la polea alta, bajar la barra hasta la altura del pecho."
            },
            new Ejercicio
            {
                Id = 7,
                Nombre = "Sentadilla",
                GrupoMuscular = "Piernas",
                Descripcion = "Con la barra en la espalda, bajar flexionando las rodillas hasta que los muslos queden paralelos al piso."
            },
            new Ejercicio
            {
                Id = 8,
                Nombre = "Peso muerto",
                GrupoMuscular = "Piernas",
                Descripcion = "Levantar la barra desde el piso con la espalda recta hasta quedar parado."
            },
            new Ejercicio
            {
                Id = 9,
                Nombre = "Prensa de piernas",
                GrupoMuscular = "Piernas",
                Descripcion = "Sentado en la máquina, empujar la plataforma con los pies."
            },
            new Ejercicio
            {
                Id = 10,
                Nombre = "Press militar",
                GrupoMuscular = "Hombros",
                Descripcion = "Parado, empujar la barra desde los hombros hasta arriba de la cabeza."
            },
            new Ejercicio
            {
                Id = 11,
                Nombre = "Elevaciones laterales",
                GrupoMuscular = "Hombros",
                Descripcion = "Con una mancuerna en cada mano, levantar los brazos por los costados hasta la altura de los hombros."
            },
            new Ejercicio
            {
                Id = 12,
                Nombre = "Curl de bíceps con barra",
                GrupoMuscular = "Brazos",
                Descripcion = "Con los codos pegados al cuerpo, flexionar los brazos llevando la barra hacia los hombros."
            },
            new Ejercicio
            {
                Id = 13,
                Nombre = "Extensión de tríceps en polea",
                GrupoMuscular = "Brazos",
                Descripcion = "Con los codos fijos, estirar los brazos empujando la soga hacia abajo."
            },
            new Ejercicio
            {
                Id = 14,
                Nombre = "Crunch abdominal",
                GrupoMuscular = "Abdominales",
                Descripcion = "Acostado boca arriba, levantar los hombros del piso contrayendo el abdomen."
            },
            new Ejercicio
            {
                Id = 15,
                Nombre = "Elevación de piernas",
                GrupoMuscular = "Abdominales",
                Descripcion = "Acostado boca arriba, levantar las piernas estiradas hasta formar un ángulo recto con el torso."
            },
        ];

        return ejercicios;
    }
}
