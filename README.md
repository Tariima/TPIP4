# TPIP4

Trabajo Práctico Integrador de Programación IV

API para llevar el seguimiento de los entrenamientos de gimnasio y reservar turnos en gimnasios adheridos.

## De qué se trata

- Cada usuario arma sus rutinas y registra sus entrenamientos (peso, series y repeticiones) para tener su historial.
- Los usuarios Pro pagan una suscripción mensual con Mercado Pago y acceden a las gráficas de progreso y a los beneficios de los gimnasios.
- Se pueden reservar turnos en los gimnasios adheridos. La reserva queda confirmada al momento y no se puede tener más de una por día.
- Cada gimnasio tiene un admin que maneja los horarios y los beneficios, y ve las reservas y estadísticas.
- El superadmin da de alta los gimnasios y aprueba las cuentas de los admins de gimnasio.
- Se mandan mails cuando se confirma o cancela una reserva, cuando se aprueba la cuenta de un admin, cuando se paga la suscripción y cuando está por vencer o vence.

## Roles

- Usuario
- Usuario Pro (usuario con la suscripción activa)
- Admin de gimnasio
- Superadmin

## Tecnologías

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core con SQL Server
- JWT para la autenticación y BCrypt para guardar las contraseñas
- Mercado Pago, consumido con HttpClientFactory y Polly
- MailKit para mandar los mails (SMTP de Gmail)
- Swagger (Swashbuckle) para probar la API
- Azure (App Service, Azure SQL y Key Vault) con deploy desde GitHub Actions

## Estructura

La solución está armada con Clean Architecture:

- `Domain`: entidades, enums, interfaces de los repositorios y excepciones
- `Application`: servicios, DTOs e interfaces
- `Infrastructure`: DbContext, migraciones, repositorios y servicios externos
- `Presentation`: la Web API (controllers, middleware y Program.cs)

Los proyectos están dentro de `src/` y la solución es `TPIP4.slnx`.

## API en Azure

La API está deployada en Azure: https://tpip4-api-fdezbhhba9czawe3.westus3-01.azurewebsites.net

Entrando a esa URL redirige directo a Swagger ([link directo](https://tpip4-api-fdezbhhba9czawe3.westus3-01.azurewebsites.net/swagger/index.html)). Si la primera request tarda o da error de timeout es porque la base de Azure SQL se pausa cuando no se usa y tarda un poco en arrancar, después anda normal.

## Cómo correrlo local

### Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server LocalDB (viene con Visual Studio 2026 si se instala la carga de trabajo de ASP.NET y desarrollo web)
- Visual Studio 2026 (opcional, también se puede correr desde la consola)

### Pasos

1. Clonar el repo:

   ```bash
   git clone https://github.com/Tariima/TPIP4.git
   cd TPIP4
   ```

2. Cargar los secretos con user-secrets. Son el token de Mercado Pago y la contraseña de la casilla de Gmail que usamos para los mails, que no están en el repo (los tiene el grupo):

   ```bash
   dotnet user-secrets set "MercadoPago:AccessToken" "el-token-de-mercado-pago" --project src/Presentation
   dotnet user-secrets set "Email:Password" "la-contraseña-de-aplicacion-de-gmail" --project src/Presentation
   ```

   Sin esto la API levanta igual y anda todo, pero no se puede pagar la suscripción y los mails no salen (el error queda en el log y la operación sigue).

3. Correr la API:
   - Desde Visual Studio: abrir `TPIP4.slnx`, dejar `Presentation` como proyecto de inicio y darle a ejecutar con el perfil `http`.
   - Desde la consola, parado en la carpeta del repo:

     ```bash
     dotnet run --project src/Presentation
     ```

4. Abrir http://localhost:5010/swagger

No hace falta crear la base a mano: cuando arranca la API se aplican las migraciones pendientes (`Database.Migrate()` en el Program.cs) y se crea la base `TPIP4Db` en LocalDB. Las migraciones ya cargan los 15 ejercicios y el usuario superadmin. Si se quiere usar otro SQL Server hay que cambiar el connection string en `src/Presentation/appsettings.Development.json`.

## Cómo probarlo

Usuario superadmin que se crea con la migración:

- Email: `superadmin@tpip4.com`
- Contraseña: `admin1234`

Con Swagger:

1. Hacer login en `POST /api/authentication/authenticate` y copiar el token que devuelve.
2. Tocar el botón **Authorize** de arriba y pegar el token (sin la palabra Bearer).
3. Probar los endpoints. El token dura 1 hora.

También está el archivo [`src/Presentation/Presentation.http`](src/Presentation/Presentation.http) con una request para cada endpoint, ordenadas para seguir el flujo completo: el superadmin crea un gimnasio, el responsable pide la cuenta, el superadmin la aprueba, el admin carga horarios y beneficios, y un usuario se registra, arma rutinas, registra entrenamientos y reserva. Se abre en Visual Studio y cada request tiene su botón para mandarla. Los tokens se pegan en las variables de arriba del archivo.

El pago de la suscripción solo se puede probar completo en Azure: Mercado Pago avisa del pago llamando al webhook (`api/pagos/webhook`) y no puede llegar a una API que corre en localhost.

## Documentación

- [Endpoints](docs/endpoints.md): qué recibe y qué devuelve cada endpoint, con los status codes.
- [Diagrama de clases](docs/diagrama-clases.md)

## Deploy

Usamos GitHub Actions con dos workflows:

- `CI`: compila la solución en cada push y en cada pull request a main.
- `Build and deploy`: cuando se mergea a main compila, publica y deploya en el App Service de Azure. Las migraciones se aplican solas al arrancar la API.

Los secretos (clave del JWT, connection string de Azure SQL, token de Mercado Pago y contraseña del mail) están cargados en Azure y no en el repo.

## Integrantes

- Gabriel Dino
- Jeremías Paletta
- Matías Sione
