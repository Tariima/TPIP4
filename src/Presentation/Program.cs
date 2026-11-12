using Application.Interfaces;
using Application.Models;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Presentation.BackgroundServices;
using Presentation.Middleware;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger con el botón Authorize para poder probar con el token
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TPIP4 API",
        Version = "v1",
        Description = "API para registrar entrenamientos y reservar turnos en gimnasios. " +
            "Para los endpoints con candado primero hay que hacer login en api/authentication/authenticate y pegar el token en Authorize."
    });

    // Los comentarios /// de los controllers se guardan en Presentation.xml al compilar
    // (GenerateDocumentationFile en el .csproj) y Swagger los muestra como descripción de cada endpoint
    string xmlPath = Path.Combine(AppContext.BaseDirectory, "Presentation.xml");
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pegá acá el token (sin la palabra Bearer)"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Configuración de la autenticación con JWT: qué valores del token se validan y contra qué
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]!))
        };
    });

// Agregar el ApplicationDbContext al contenedor de servicios
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
// EnableRetryOnFailure reintenta la conexión si la base de Azure está pausada y tarda en despertar
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));

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
builder.Services.AddScoped<IProgresoService, ProgresoService>();
// Mails por SMTP con Gmail (MailKit). La contraseña no está en appsettings: va en user-secrets o en variables de entorno de Azure
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICustomAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
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

// El monto de la suscripción se lee de appsettings y se inyecta en el service como un objeto común
builder.Services.AddSingleton(builder.Configuration.GetSection("Suscripcion").Get<SuscripcionSettings>()!);

// Cliente HTTP para Mercado Pago con HttpClientFactory. La factory maneja la vida de las conexiones,
// y el resilience handler (Polly) reintenta si Mercado Pago falla o no responde
builder.Services.AddHttpClient<IMercadoPagoService, MercadoPagoService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MercadoPago:BaseUrl"]!);
    // El token no está en appsettings: se carga con user-secrets (local) o variables de entorno (Azure)
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", builder.Configuration["MercadoPago:AccessToken"]);
})
.AddResilienceHandler("mercadopago", pipeline =>
{
    // Hasta 3 reintentos ante errores temporales (5xx, 408, 429 o sin conexión),
    // esperando cada vez más entre uno y otro (1s, 2s, 4s)
    pipeline.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential
    });

    // Cada intento corta a los 10 segundos para no dejar colgado al usuario
    pipeline.AddTimeout(TimeSpan.FromSeconds(10));
});

// Proceso en segundo plano que vence las suscripciones una vez por día
builder.Services.AddHostedService<VencimientoSuscripcionesService>();

var app = builder.Build();

// Aplica las migraciones pendientes al arrancar, así en Azure se crean las tablas en cada deploy
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger habilitado también en producción, para poder probar la API desplegada en Azure
app.UseSwagger();
app.UseSwaggerUI();

// La raíz redirige a Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
