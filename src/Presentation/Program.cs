using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar el ApplicationDbContext al contenedor de servicios
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configurar servicios y repositorios como Scoped
#region Services
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEjercicioService, EjercicioService>();
builder.Services.AddScoped<IRutinaService, RutinaService>();
builder.Services.AddScoped<IEntrenamientoService, EntrenamientoService>();
builder.Services.AddScoped<ISuscripcionService, SuscripcionService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<IGimnasioService, GimnasioService>();
builder.Services.AddScoped<ISolicitudCuentaGimnasioService, SolicitudCuentaGimnasioService>();
builder.Services.AddScoped<IHorarioService, HorarioService>();
builder.Services.AddScoped<IBeneficioService, BeneficioService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
#endregion

#region Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEjercicioRepository, EjercicioRepository>();
builder.Services.AddScoped<IRutinaRepository, RutinaRepository>();
builder.Services.AddScoped<IEntrenamientoRepository, EntrenamientoRepository>();
builder.Services.AddScoped<ISuscripcionRepository, SuscripcionRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IGimnasioRepository, GimnasioRepository>();
builder.Services.AddScoped<ISolicitudCuentaGimnasioRepository, SolicitudCuentaGimnasioRepository>();
builder.Services.AddScoped<IHorarioRepository, HorarioRepository>();
builder.Services.AddScoped<IBeneficioRepository, BeneficioRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
#endregion

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
