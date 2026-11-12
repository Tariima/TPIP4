# Diagrama de clases

Diagrama de las entidades de `Domain/Entities`, como quedaron en el código. Lo hicimos con Mermaid para que GitHub lo muestre directo (si no se ve, se puede pegar el bloque en https://mermaid.live).

No pusimos las foreign keys (`GimnasioId`, `RutinaId`, etc.) ni las listas de navegación porque ya se ven en las relaciones.

```mermaid
classDiagram
    direction TB

    class Usuario {
        <<abstract>>
        +int Id
        +string Nombre
        +string Apellido
        +string Email
        +string Contrasena
        +DateTime FechaRegistro
        +bool Activo
    }

    class UsuarioComun {
        +DateTime FechaNacimiento
        +float Peso
        +float Altura
        +int PuntajeTotal
    }

    class AdminGimnasio {
        +DateTime? FechaAprobacion
    }

    class SuperAdmin

    class Gimnasio {
        +int Id
        +string Nombre
        +string Direccion
        +string Telefono
        +string Email
        +DateTime FechaAlta
        +bool Activo
    }

    class SolicitudCuentaGimnasio {
        +int Id
        +string NombreSolicitante
        +string EmailSolicitante
        +DateTime FechaSolicitud
        +EstadoSolicitud Estado
        +DateTime? FechaResolucion
    }

    class Horario {
        +int Id
        +string DiaSemana
        +DateTime HoraInicio
        +DateTime HoraFin
        +int CupoMaximo
        +bool Activo
    }

    class Reserva {
        +int Id
        +DateTime FechaTurno
        +DateTime FechaSolicitud
        +DateTime? FechaCancelacion
        +EstadoReserva Estado
    }

    class Beneficio {
        +int Id
        +string Titulo
        +string Descripcion
        +string Tipo
        +float PorcentajeDescuento
        +DateTime VigenteDesde
        +DateTime VigenteHasta
        +bool Activo
    }

    class Ejercicio {
        +int Id
        +string Nombre
        +string GrupoMuscular
        +string Descripcion
    }

    class Rutina {
        +int Id
        +string Nombre
        +string Descripcion
        +DateTime FechaCreacion
        +bool Activa
    }

    class EjercicioRutina {
        +int Id
        +int Orden
        +int SeriesObjetivo
        +int RepeticionesObjetivo
    }

    class Entrenamiento {
        +int Id
        +DateTime Fecha
        +int DuracionMinutos
        +string Observaciones
    }

    class RegistroSerie {
        +int Id
        +int NumeroSerie
        +float Peso
        +int Repeticiones
    }

    class Suscripcion {
        +int Id
        +DateTime FechaInicio
        +DateTime FechaVencimiento
        +decimal MontoMensual
        +EstadoSuscripcion Estado
    }

    class Pago {
        +int Id
        +DateTime Fecha
        +decimal Monto
        +string MedioPago
        +EstadoPago Estado
        +string? IdExterno
    }

    class EstadoSolicitud {
        <<enumeration>>
        Pendiente
        Aprobada
        Rechazada
    }

    class EstadoReserva {
        <<enumeration>>
        Confirmada
        Cancelada
    }

    class EstadoSuscripcion {
        <<enumeration>>
        Pendiente
        Activa
        Vencida
    }

    class EstadoPago {
        <<enumeration>>
        Pendiente
        Aprobado
        Rechazado
    }

    Usuario <|-- UsuarioComun
    Usuario <|-- AdminGimnasio
    Usuario <|-- SuperAdmin

    SuperAdmin "1" -- "0..*" Gimnasio : da de alta
    SuperAdmin "0..1" -- "0..*" SolicitudCuentaGimnasio : resuelve
    Gimnasio "1" -- "0..*" SolicitudCuentaGimnasio : recibe
    Gimnasio "1" -- "0..*" AdminGimnasio : administra
    Gimnasio "1" -- "0..*" Horario : tiene
    Gimnasio "1" -- "0..*" Beneficio : ofrece
    Horario "1" -- "0..*" Reserva : tiene
    UsuarioComun "1" -- "0..*" Reserva : hace

    UsuarioComun "1" -- "0..*" Rutina : arma
    Rutina "1" -- "1..*" EjercicioRutina : contiene
    Ejercicio "1" -- "0..*" EjercicioRutina : aparece en
    UsuarioComun "1" -- "0..*" Entrenamiento : registra
    Rutina "0..1" -- "0..*" Entrenamiento : se basa en
    Entrenamiento "1" -- "1..*" RegistroSerie : tiene
    Ejercicio "1" -- "0..*" RegistroSerie : de

    UsuarioComun "1" -- "0..*" Suscripcion : contrata
    Suscripcion "1" -- "0..*" Pago : tiene
```

## Algunas aclaraciones

- `Usuario` es abstracta y las tres clases hijas se guardan en la misma tabla (TPH). EF agrega la columna `Discriminator` para saber de qué tipo es cada fila, y ese mismo nombre de clase es el rol que va en el token.
- `EjercicioRutina` es la clase intermedia entre `Rutina` y `Ejercicio`, porque además de la relación guarda el orden y las series / repeticiones objetivo.
- En `Horario`, `HoraInicio` y `HoraFin` son `DateTime` pero solo se usa la hora. En la API se mandan y se devuelven como texto `HH:mm`.
- `Pago.IdExterno` es el id de la orden en Mercado Pago. Queda en null hasta que se crea la orden.
- Las bajas de gimnasios, horarios, beneficios y rutinas son lógicas (`Activo` / `Activa` en false). Los entrenamientos sí se borran.
