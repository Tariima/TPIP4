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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger con el botón Authorize para probar con token
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kineo API",
        Version = "v1",
        Description = "API para registrar entrenamientos y reservar turnos en gimnasios. " +
            "Para los endpoints con candado primero hay que hacer login en api/authentication/authenticate y pegar el token en Authorize."
    });

    // Los /// de los controllers van a Presentation.xml (GenerateDocumentationFile) y Swagger los muestra
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

// JWT: qué se valida del token
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

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
// reintenta si la base de Azure está pausada y tarda en arrancar
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Services y repositorios (scoped)
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
// Mails por SMTP (MailKit + Gmail). La contraseña va en user-secrets o en las variables de entorno de Azure
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

// monto de la suscripción, sale del appsettings
builder.Services.AddSingleton(builder.Configuration.GetSection("Suscripcion").Get<SuscripcionSettings>()!);

// HttpClient de MP con HttpClientFactory, y Polly para los reintentos
builder.Services.AddHttpClient<IMercadoPagoService, MercadoPagoService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MercadoPago:BaseUrl"]!);
    // el token va en user-secrets (local) o en variables de entorno (Azure), nunca en appsettings
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", builder.Configuration["MercadoPago:AccessToken"]);
})
.AddResilienceHandler("mercadopago", pipeline =>
{
    // hasta 3 reintentos con espera exponencial (aprox. 1s, 2s, 4s) ante 5xx, 408, 429 o error de red
    pipeline.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential
    });

    // timeout de 10 seg por intento
    pipeline.AddTimeout(TimeSpan.FromSeconds(10));
});

// job diario de vencimientos
builder.Services.AddHostedService<VencimientoSuscripcionesService>();

var app = builder.Build();

// aplica las migraciones pendientes al arrancar, así Azure queda al día en cada deploy
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger también en producción, para probar en Azure
app.UseSwagger();
app.UseSwaggerUI();

// / redirige a Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
