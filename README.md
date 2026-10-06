# TPIP4

Trabajo Práctico Integrador de Programación IV (TUP - UTN).

API para llevar el seguimiento de los entrenamientos de gimnasio y reservar turnos en gimnasios adheridos.

## De qué se trata

- Cada usuario arma sus rutinas y registra sus entrenamientos (peso, series y repeticiones) para tener su historial.
- Los usuarios Pro pagan una suscripción mensual con Mercado Pago y acceden a las gráficas de progreso y a los beneficios de los gimnasios.
- Se pueden reservar turnos en los gimnasios adheridos. La reserva queda confirmada al momento y no se puede tener más de una por día.
- Cada gimnasio tiene un admin que maneja los horarios y los beneficios, y ve las reservas y estadísticas.
- El superadmin da de alta los gimnasios y aprueba las cuentas de los admins de gimnasio.
- Se mandan mails cuando se confirma o cancela una reserva, cuando se paga la suscripción y cuando está por vencer.

## Roles

- Usuario
- Usuario Pro (usuario con la suscripción activa)
- Admin de gimnasio
- Superadmin

## Tecnologías

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core con SQL Server
- JWT para la autenticación
- Mercado Pago, consumido con HttpClientFactory y Polly
- Azure (App Service, Azure SQL y Key Vault) con deploy desde GitHub Actions

## Estructura

La solución está armada con Clean Architecture:

- `Domain`: entidades, enums, interfaces de los repositorios y excepciones
- `Application`: servicios, DTOs e interfaces
- `Infrastructure`: DbContext, migraciones, repositorios y servicios externos
- `Presentation`: la Web API (controllers, middleware y Program.cs)

## Deploy

Pendiente. Cuando esté, acá va el link de la API en Azure.

## Integrantes

- Gabriel Dino
- Jeremías Paletta
- Matías Sione
