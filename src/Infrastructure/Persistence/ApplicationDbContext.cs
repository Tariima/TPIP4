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
        // Composiciones del diagrama: si se borra el todo se borran sus partes
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

        // Asociaciones: no se borran en cascada
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

        base.OnModelCreating(modelBuilder);
    }
}
