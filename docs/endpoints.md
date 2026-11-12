# Documentación de los endpoints

Seguimos el formato que vimos en la práctica de APIs: para cada endpoint el método y la ruta, qué hace, qué recibe (request) y qué devuelve (response) con los status codes.

Los modelos (DTOs) que se repiten están explicados al final, en [Modelos](#modelos).

## Cosas generales

- Todo va y vuelve en JSON.
- Las fechas van con formato `yyyy-MM-dd` (ej: `2026-11-16`). Las horas de los horarios van como texto `HH:mm` (ej: `08:30`).
- Los endpoints que piden token llevan el header `Authorization: Bearer <token>`. El token se saca del login y dura 1 hora.
- Roles: `UsuarioComun`, `AdminGimnasio` y `SuperAdmin`. Un usuario Pro es un `UsuarioComun` con la suscripción activa, no es otro rol.
- Cuando algo falla la API devuelve `{ "error": "mensaje" }` con el status code que corresponde (lo arma el middleware de excepciones).
- Si falta un campo requerido o un email no es válido, ASP.NET devuelve 400 solo, con la lista de errores en `errors`.

Status codes que pueden aparecer en cualquier endpoint con token y no los repetimos en cada uno:

| Status | Cuándo |
|---|---|
| 401 | No se mandó el token o ya venció |
| 403 | El token es de un rol que no puede usar ese endpoint |
| 500 | Error inesperado |

---

## Autenticación

### POST /api/authentication/authenticate
Login. No pide token.

**Request**

Body: [CredentialsDTO](#credentialsdto)

**Response**
- 200: el token JWT (texto). Adentro lleva el id del usuario (`sub`), el nombre (`given_name`) y el rol (`role`)
- 400: falta el email o la contraseña
- 401: el email o la contraseña están mal, o la cuenta está inactiva (por ejemplo un admin de gimnasio que todavía no fue aprobado)

---

## Usuarios

### POST /api/usuarios
Registro de un usuario nuevo. No pide token.

**Request**

Body: [CreationUsuarioDTO](#creationusuariodto)

**Response**
- 201: [UsuarioDTO](#usuariodto)
- 400: faltan datos, el email no es válido o el peso / altura no son mayores a cero
- 409: ya existe un usuario con ese email

### GET /api/usuarios/perfil
Perfil del usuario logueado. Rol: `UsuarioComun`.

**Request:** sin parámetros

**Response**
- 200: [UsuarioComunDTO](#usuariocomundto)
- 404: no se encontró el usuario

### PUT /api/usuarios/perfil
Modifica los datos del perfil. El email no se puede cambiar. Rol: `UsuarioComun`.

**Request**

Body: [UpdateUsuarioDTO](#updateusuariodto)

**Response**
- 204: se modificó
- 400: faltan datos o el peso / altura no son mayores a cero
- 404: no se encontró el usuario

### PUT /api/usuarios/perfil/contrasena
Cambia la contraseña. Rol: `UsuarioComun`.

**Request**

Body:

| Nombre | Tipo | Descripción |
|---|---|---|
| contrasenaActual | string | Requerido |
| contrasenaNueva | string | Requerido |

**Response**
- 204: se cambió
- 400: la contraseña actual es incorrecta
- 404: no se encontró el usuario

---

## Ejercicios

Son los ejercicios que se cargan con la migración (15, de Pecho, Espalda, Piernas, Hombros, Brazos y Abdominales). Rol: `UsuarioComun`.

### GET /api/ejercicios
Lista los ejercicios.

**Request**

Query:

| Nombre | Tipo | Descripción |
|---|---|---|
| grupoMuscular | string | Opcional. Ej: `Pecho` |

**Response**
- 200: lista de [EjercicioDTO](#ejerciciodto)
- 404: no hay ejercicios de ese grupo muscular

### GET /api/ejercicios/{id}
Un ejercicio.

**Request**

Path: `id` (int) id del ejercicio

**Response**
- 200: [EjercicioDTO](#ejerciciodto)
- 404: no existe el ejercicio

---

## Rutinas

Cada usuario ve solo sus rutinas. Si se pide una de otro usuario da 404. Rol: `UsuarioComun`.

### GET /api/rutinas
Las rutinas activas del usuario.

**Request:** sin parámetros

**Response**
- 200: lista de [RutinaDTO](#rutinadto)

### GET /api/rutinas/{id}
Una rutina con sus ejercicios.

**Request**

Path: `id` (int) id de la rutina

**Response**
- 200: [RutinaDTO](#rutinadto)
- 404: no existe, es de otro usuario o está dada de baja

### POST /api/rutinas
Crea una rutina.

**Request**

Body: [CreationRutinaDTO](#creationrutinadto)

**Response**
- 201: [RutinaDTO](#rutinadto)
- 400: falta el nombre, no tiene ejercicios o las series / repeticiones objetivo no son mayores a cero
- 404: alguno de los ejercicios no existe

### PUT /api/rutinas/{id}
Modifica una rutina. La lista de ejercicios que se manda reemplaza a la que tenía.

**Request**

Path: `id` (int) id de la rutina

Body: [CreationRutinaDTO](#creationrutinadto) (es el mismo formato, se llama UpdateRutinaDTO)

**Response**
- 204: se modificó
- 400: igual que en el POST
- 404: no existe la rutina o alguno de los ejercicios

### DELETE /api/rutinas/{id}
Baja lógica de la rutina (queda con `Activa = false`). Los entrenamientos que la usaban no se borran.

**Request**

Path: `id` (int) id de la rutina

**Response**
- 204: se dio de baja
- 404: no existe la rutina

---

## Entrenamientos

Cada usuario ve solo sus entrenamientos. Rol: `UsuarioComun`.

### GET /api/entrenamientos
Historial de entrenamientos, del más nuevo al más viejo.

**Request**

Query:

| Nombre | Tipo | Descripción |
|---|---|---|
| desde | fecha | Opcional |
| hasta | fecha | Opcional |

**Response**
- 200: lista de [EntrenamientoDTO](#entrenamientodto)
- 400: la fecha desde es posterior a la fecha hasta

### GET /api/entrenamientos/{id}
Un entrenamiento con sus series.

**Request**

Path: `id` (int) id del entrenamiento

**Response**
- 200: [EntrenamientoDTO](#entrenamientodto)
- 404: no existe o es de otro usuario

### POST /api/entrenamientos
Registra un entrenamiento.

**Request**

Body: [CreationEntrenamientoDTO](#creationentrenamientodto)

**Response**
- 201: [EntrenamientoDTO](#entrenamientodto)
- 400: la fecha es futura, la duración no es mayor a cero, no tiene series, o alguna serie tiene número / repeticiones en 0 o peso negativo
- 404: no existe la rutina o alguno de los ejercicios

### PUT /api/entrenamientos/{id}
Modifica un entrenamiento. Las series que se mandan reemplazan a las que tenía.

**Request**

Path: `id` (int) id del entrenamiento

Body: [CreationEntrenamientoDTO](#creationentrenamientodto) (mismo formato, se llama UpdateEntrenamientoDTO)

**Response**
- 204: se modificó
- 400: igual que en el POST
- 404: no existe el entrenamiento, la rutina o alguno de los ejercicios

### DELETE /api/entrenamientos/{id}
Borra el entrenamiento con sus series (este sí se borra de la base).

**Request**

Path: `id` (int) id del entrenamiento

**Response**
- 204: se borró
- 404: no existe el entrenamiento

---

## Progreso (solo Pro)

Datos para armar las gráficas. Rol: `UsuarioComun` con la suscripción activa.

### GET /api/progreso/ejercicios/{idEjercicio}
Peso máximo y volumen de cada día que se hizo ese ejercicio.

**Request**

Path: `idEjercicio` (int)

Query: `desde` y `hasta` (fechas, opcionales)

**Response**
- 200: lista de [ProgresoEjercicioDTO](#progresoejerciciodto)
- 400: la fecha desde es posterior a la fecha hasta
- 403: el usuario no es Pro
- 404: no existe el ejercicio

### GET /api/progreso/semanal
Resumen por semana.

**Request**

Query: `desde` y `hasta` (fechas, opcionales)

**Response**
- 200: lista de [ResumenSemanalDTO](#resumensemanaldto)
- 400: la fecha desde es posterior a la fecha hasta
- 403: el usuario no es Pro

---

## Suscripciones

Rol: `UsuarioComun`.

### GET /api/suscripciones
La suscripción del usuario con sus pagos.

**Request:** sin parámetros

**Response**
- 200: [SuscripcionDTO](#suscripciondto)
- 404: el usuario nunca contrató Pro, no tiene suscripción

### POST /api/suscripciones/pagar
Sirve para contratar Pro y para renovar. Crea el pago como pendiente y la orden en Mercado Pago, y devuelve el link para pagar un mes. La suscripción se activa (o se extiende un mes) cuando Mercado Pago avisa al webhook que se pagó.

**Request:** sin parámetros

**Response**
- 200: [LinkPagoDTO](#linkpagodto)
- 500: Mercado Pago no respondió (después de los reintentos). El pago queda como rechazado

---

## Pagos

### POST /api/pagos/webhook
Lo llama Mercado Pago cuando cambia el estado de una orden, no es para usarlo desde el front. No pide token. Con el id que llega se consulta la orden a Mercado Pago, y si se pagó se aprueba el pago, se activa la suscripción y se manda un mail.

**Request**

Body:

```json
{
  "action": "order.processed",
  "type": "order",
  "data": { "id": "ORD01JYH1Z1YJN4HZ8J3Q0RB3YP6D" }
}
```

**Response**
- 200: siempre que se pudo procesar, aunque el aviso no sirva (otro tipo, orden que no existe, pago ya procesado). Si devolviera error Mercado Pago lo vuelve a mandar

---

## Gimnasios

### GET /api/gimnasios
Gimnasios activos. No pide token (lo usa el responsable de un gimnasio para elegirlo cuando pide la cuenta).

**Request:** sin parámetros

**Response**
- 200: lista de [GimnasioDTO](#gimnasiodto)

### GET /api/gimnasios/todos
Todos los gimnasios, también los dados de baja. Rol: `SuperAdmin`.

**Request:** sin parámetros

**Response**
- 200: lista de [GimnasioDTO](#gimnasiodto)

### GET /api/gimnasios/{id}
Un gimnasio activo. Rol: `SuperAdmin` o `UsuarioComun`.

**Request**

Path: `id` (int) id del gimnasio

**Response**
- 200: [GimnasioDTO](#gimnasiodto)
- 404: no existe o está dado de baja

### POST /api/gimnasios
Alta de un gimnasio. Rol: `SuperAdmin`.

**Request**

Body: [CreationGimnasioDTO](#creationgimnasiodto)

**Response**
- 201: [GimnasioDTO](#gimnasiodto)
- 400: faltan datos o el email no es válido

### PUT /api/gimnasios/{id}
Modifica un gimnasio. Rol: `SuperAdmin`.

**Request**

Path: `id` (int) id del gimnasio

Body: [CreationGimnasioDTO](#creationgimnasiodto)

**Response**
- 204: se modificó
- 400: faltan datos o el email no es válido
- 404: no existe o está dado de baja

### DELETE /api/gimnasios/{id}
Baja lógica del gimnasio. También desactiva sus horarios y beneficios. Rol: `SuperAdmin`.

**Request**

Path: `id` (int) id del gimnasio

**Response**
- 204: se dio de baja
- 404: no existe o ya estaba dado de baja

---

## Solicitudes de cuenta de gimnasio

El responsable de un gimnasio pide su cuenta, la cuenta se crea inactiva y el superadmin la aprueba o la rechaza.

### POST /api/solicitudescuentagimnasio
Pedir la cuenta de admin de un gimnasio. No pide token.

**Request**

Body: [CreationSolicitudCuentaGimnasioDTO](#creationsolicitudcuentagimnasiodto)

**Response**
- 201: [SolicitudCuentaGimnasioDTO](#solicitudcuentagimnasiodto)
- 400: faltan datos o el email no es válido
- 404: no existe el gimnasio
- 409: ya existe un usuario con ese email

### GET /api/solicitudescuentagimnasio
Lista las solicitudes. Rol: `SuperAdmin`.

**Request**

Query:

| Nombre | Tipo | Descripción |
|---|---|---|
| estado | string | Opcional. `Pendiente`, `Aprobada` o `Rechazada` |

**Response**
- 200: lista de [SolicitudCuentaGimnasioDTO](#solicitudcuentagimnasiodto)
- 400: el estado no es ninguno de los tres

### GET /api/solicitudescuentagimnasio/{id}
Una solicitud. Rol: `SuperAdmin`.

**Request**

Path: `id` (int) id de la solicitud

**Response**
- 200: [SolicitudCuentaGimnasioDTO](#solicitudcuentagimnasiodto)
- 404: no existe la solicitud

### PATCH /api/solicitudescuentagimnasio/{id}/aprobar
Aprueba la solicitud, activa la cuenta del admin y le manda un mail. Rol: `SuperAdmin`.

**Request**

Path: `id` (int) id de la solicitud

**Response**
- 204: se aprobó
- 404: no existe la solicitud o la cuenta del solicitante
- 409: la solicitud ya fue aprobada o rechazada, o el gimnasio está dado de baja

### PATCH /api/solicitudescuentagimnasio/{id}/rechazar
Rechaza la solicitud. La cuenta queda inactiva. Rol: `SuperAdmin`.

**Request**

Path: `id` (int) id de la solicitud

**Response**
- 204: se rechazó
- 404: no existe la solicitud
- 409: la solicitud ya fue aprobada o rechazada

---

## Horarios

El admin maneja los horarios de su gimnasio (el gimnasio sale del token, no se manda). Rol: `AdminGimnasio`.

### GET /api/horarios
Los horarios del gimnasio del admin.

**Request:** sin parámetros

**Response**
- 200: lista de [HorarioDTO](#horariodto)

### GET /api/horarios/{id}
Un horario del gimnasio.

**Request**

Path: `id` (int) id del horario

**Response**
- 200: [HorarioDTO](#horariodto)
- 404: no existe o es de otro gimnasio

### POST /api/horarios
Crea un horario.

**Request**

Body: [CreationHorarioDTO](#creationhorariodto)

**Response**
- 201: [HorarioDTO](#horariodto)
- 400: el día no es de Lunes a Domingo, las horas no tienen formato `HH:mm`, la hora de inicio no es menor a la de fin o el cupo no es mayor a cero
- 409: el gimnasio está dado de baja

### PUT /api/horarios/{id}
Modifica un horario.

**Request**

Path: `id` (int) id del horario

Body: [CreationHorarioDTO](#creationhorariodto)

**Response**
- 204: se modificó
- 400: igual que en el POST
- 404: no existe o es de otro gimnasio
- 409: el gimnasio está dado de baja

### DELETE /api/horarios/{id}
Desactiva el horario (baja lógica). Las reservas que ya tenía no se borran.

**Request**

Path: `id` (int) id del horario

**Response**
- 204: se desactivó
- 404: no existe o es de otro gimnasio

---

## Beneficios

Los carga el admin de cada gimnasio y los ven los usuarios Pro.

### GET /api/beneficios/gimnasio
Los beneficios del gimnasio del admin, vigentes o no. Rol: `AdminGimnasio`.

**Request:** sin parámetros

**Response**
- 200: lista de [BeneficioDTO](#beneficiodto)

### GET /api/beneficios/vigentes
Los gimnasios que hoy tienen beneficios vigentes, cada uno con los suyos. Rol: `UsuarioComun` Pro.

**Request:** sin parámetros

**Response**
- 200: lista de [GimnasioConBeneficiosDTO](#gimnasioconbeneficiosdto)
- 403: el usuario no es Pro

### GET /api/beneficios/{id}
Un beneficio del gimnasio del admin. Rol: `AdminGimnasio`.

**Request**

Path: `id` (int) id del beneficio

**Response**
- 200: [BeneficioDTO](#beneficiodto)
- 404: no existe o es de otro gimnasio

### POST /api/beneficios
Carga un beneficio. Rol: `AdminGimnasio`.

**Request**

Body: [CreationBeneficioDTO](#creationbeneficiodto)

**Response**
- 201: [BeneficioDTO](#beneficiodto)
- 400: el porcentaje no está entre 0 y 100, la fecha de inicio es posterior a la de fin o la de fin ya pasó
- 409: el gimnasio está dado de baja

### PUT /api/beneficios/{id}
Modifica un beneficio. Rol: `AdminGimnasio`.

**Request**

Path: `id` (int) id del beneficio

Body: [CreationBeneficioDTO](#creationbeneficiodto)

**Response**
- 204: se modificó
- 400: el porcentaje no está entre 0 y 100 o la fecha de inicio es posterior a la de fin
- 404: no existe o es de otro gimnasio
- 409: el gimnasio está dado de baja

### DELETE /api/beneficios/{id}
Baja lógica del beneficio. Rol: `AdminGimnasio`.

**Request**

Path: `id` (int) id del beneficio

**Response**
- 204: se dio de baja
- 404: no existe o es de otro gimnasio

---

## Reservas

Reservar y cancelar es del `UsuarioComun` (no hace falta ser Pro). Ver las reservas del gimnasio y las estadísticas es del `AdminGimnasio`.

Reglas: la reserva queda confirmada al momento, no se puede tener más de una confirmada por día, solo se puede cancelar antes de que empiece el turno y cuando se confirma o se cancela llega un mail.

### GET /api/reservas/disponibles
Los horarios de un gimnasio para una fecha que todavía tienen lugar. Los que no tienen cupo o ya empezaron no aparecen. Rol: `UsuarioComun`.

**Request**

Query:

| Nombre | Tipo | Descripción |
|---|---|---|
| gimnasioId | int | Requerido |
| fecha | fecha | Requerido. No puede ser anterior a hoy |

**Response**
- 200: lista de [HorarioDisponibleDTO](#horariodisponibledto)
- 400: la fecha ya pasó
- 404: no existe el gimnasio o está dado de baja

### GET /api/reservas
Las reservas del usuario logueado. Rol: `UsuarioComun`.

**Request:** sin parámetros

**Response**
- 200: lista de [ReservaDTO](#reservadto)

### GET /api/reservas/{id}
Una reserva del usuario. Rol: `UsuarioComun`.

**Request**

Path: `id` (int) id de la reserva

**Response**
- 200: [ReservaDTO](#reservadto)
- 404: no existe o es de otro usuario

### POST /api/reservas
Reserva un turno. Rol: `UsuarioComun`.

**Request**

Body:

| Nombre | Tipo | Descripción |
|---|---|---|
| horarioId | int | Requerido |
| fecha | fecha | Requerido. Solo importa el día, la hora sale del horario |

**Response**
- 201: [ReservaDTO](#reservadto)
- 400: el día de la fecha no coincide con el del horario, o el turno ya pasó
- 404: no existe el horario
- 409: ya tiene una reserva confirmada ese día, o el horario no tiene cupo

### PATCH /api/reservas/{id}/cancelar
Cancela una reserva. Rol: `UsuarioComun`.

**Request**

Path: `id` (int) id de la reserva

**Response**
- 204: se canceló
- 400: el turno ya empezó o ya pasó
- 404: no existe o es de otro usuario
- 409: ya estaba cancelada

### GET /api/reservas/gimnasio
Las reservas del gimnasio del admin para un día. Rol: `AdminGimnasio`.

**Request**

Query:

| Nombre | Tipo | Descripción |
|---|---|---|
| fecha | fecha | Requerido |
| horarioId | int | Opcional, para ver un solo horario |

**Response**
- 200: lista de [ReservaGimnasioDTO](#reservagimnasiodto)
- 400: no se mandó la fecha

### GET /api/reservas/estadisticas
Estadísticas de un período. Rol: `AdminGimnasio`.

La ocupación de cada horario es: reservas confirmadas / (cupo x cantidad de veces que cae ese día en el período). Por ejemplo un horario de los jueves con cupo 10 en un mes con 4 jueves ofrece 40 lugares.

**Request**

Query:

| Nombre | Tipo | Descripción |
|---|---|---|
| desde | fecha | Requerido |
| hasta | fecha | Requerido. Máximo un año después de desde |

**Response**
- 200: [EstadisticasGimnasioDTO](#estadisticasgimnasiodto)
- 400: falta alguna fecha, desde es mayor a hasta o el período es de más de un año

---

## Procesos automáticos

No son endpoints pero los dejamos anotados. Hay un BackgroundService (`VencimientoSuscripcionesService`) que corre cuando arranca la API y después una vez por día:

- Manda un mail a las suscripciones que vencen en 3 días.
- Pasa a `Vencida` las que ya pasaron la fecha de vencimiento y les manda un mail.

---

## Modelos

### Request

#### CredentialsDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| email | string | Requerido |
| contrasena | string | Requerido |

#### CreationUsuarioDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| nombre | string | Requerido |
| apellido | string | Requerido |
| email | string | Requerido, tiene que ser un email válido y no estar registrado |
| contrasena | string | Requerido. Se guarda hasheada con BCrypt |
| fechaNacimiento | fecha | Requerido |
| peso | float | Requerido, mayor a cero |
| altura | float | Requerido, mayor a cero |

#### UpdateUsuarioDTO

Igual que CreationUsuarioDTO pero sin email ni contraseña.

#### CreationRutinaDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| nombre | string | Requerido |
| descripcion | string | |
| ejercicios | array | Al menos uno, con la estructura de abajo |
| - ejercicioId | int | Requerido |
| - orden | int | Requerido |
| - seriesObjetivo | int | Requerido, mayor a cero |
| - repeticionesObjetivo | int | Requerido, mayor a cero |

#### CreationEntrenamientoDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| fecha | fecha | Requerido, no puede ser futura |
| duracionMinutos | int | Requerido, mayor a cero |
| observaciones | string | |
| rutinaId | int | Opcional, solo si se hizo una rutina |
| series | array | Al menos una, con la estructura de abajo |
| - ejercicioId | int | Requerido |
| - numeroSerie | int | Requerido, mayor a cero |
| - peso | float | Requerido, no negativo (0 para ejercicios sin peso) |
| - repeticiones | int | Requerido, mayor a cero |

#### CreationGimnasioDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| nombre | string | Requerido |
| direccion | string | Requerido |
| telefono | string | Requerido |
| email | string | Requerido, tiene que ser un email válido |

#### CreationSolicitudCuentaGimnasioDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| gimnasioId | int | Requerido |
| nombre | string | Requerido |
| apellido | string | Requerido |
| email | string | Requerido, es con el que después hace login |
| contrasena | string | Requerido |

#### CreationHorarioDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| diaSemana | string | Requerido. `Lunes` a `Domingo` |
| horaInicio | string | Requerido. Formato `HH:mm` |
| horaFin | string | Requerido. Formato `HH:mm`, mayor a horaInicio |
| cupoMaximo | int | Requerido, mayor a cero |

#### CreationBeneficioDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| titulo | string | Requerido |
| descripcion | string | |
| tipo | string | Texto libre, ej: `Descuento en cuota`, `Clase gratis` |
| porcentajeDescuento | float | Requerido, entre 0 y 100 |
| vigenteDesde | fecha | Requerido |
| vigenteHasta | fecha | Requerido. Al crear no puede ser anterior a hoy |

### Response

#### UsuarioDTO

| Nombre | Tipo |
|---|---|
| id | int |
| nombre | string |
| apellido | string |
| email | string |
| fechaRegistro | fecha |
| activo | bool |

#### UsuarioComunDTO

Lo mismo que UsuarioDTO más `fechaNacimiento`, `peso`, `altura` y `puntajeTotal`.

#### EjercicioDTO

| Nombre | Tipo |
|---|---|
| id | int |
| nombre | string |
| grupoMuscular | string |
| descripcion | string |

#### RutinaDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| id | int | |
| nombre | string | |
| descripcion | string | |
| fechaCreacion | fecha | |
| activa | bool | |
| ejercicios | array | Ordenados por `orden` |
| - ejercicioId | int | |
| - nombreEjercicio | string | |
| - orden | int | |
| - seriesObjetivo | int | |
| - repeticionesObjetivo | int | |

#### EntrenamientoDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| id | int | |
| fecha | fecha | |
| duracionMinutos | int | |
| observaciones | string | |
| rutinaId | int | null si no se hizo una rutina |
| nombreRutina | string | null si no se hizo una rutina |
| series | array | Agrupadas por ejercicio |
| - ejercicioId | int | |
| - nombreEjercicio | string | |
| - numeroSerie | int | |
| - peso | float | |
| - repeticiones | int | |

#### ProgresoEjercicioDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| fecha | fecha | |
| pesoMaximo | float | El peso más alto de ese día |
| volumen | float | Suma de peso x repeticiones de las series del día |

#### ResumenSemanalDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| inicioSemana | fecha | El lunes de esa semana |
| cantidadEntrenamientos | int | |
| minutosTotales | int | |
| volumenTotal | float | |

#### SuscripcionDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| estado | string | `Pendiente`, `Activa` o `Vencida` |
| fechaInicio | fecha | |
| fechaVencimiento | fecha | |
| montoMensual | decimal | |
| pagos | array | Del más nuevo al más viejo |
| - id | int | |
| - fecha | fecha | |
| - monto | decimal | |
| - medioPago | string | |
| - estado | string | `Pendiente`, `Aprobado` o `Rechazado` |

#### LinkPagoDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| idPago | int | |
| monto | decimal | |
| linkPago | string | Link de Mercado Pago para pagar |

#### GimnasioDTO

| Nombre | Tipo |
|---|---|
| id | int |
| nombre | string |
| direccion | string |
| telefono | string |
| email | string |
| fechaAlta | fecha |
| activo | bool |

#### SolicitudCuentaGimnasioDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| id | int | |
| nombreSolicitante | string | |
| emailSolicitante | string | |
| fechaSolicitud | fecha | |
| estado | string | `Pendiente`, `Aprobada` o `Rechazada` |
| fechaResolucion | fecha | null mientras está pendiente |
| gimnasioId | int | |
| nombreGimnasio | string | |

#### HorarioDTO

| Nombre | Tipo |
|---|---|
| id | int |
| diaSemana | string |
| horaInicio | string (`HH:mm`) |
| horaFin | string (`HH:mm`) |
| cupoMaximo | int |
| activo | bool |
| gimnasioId | int |

#### HorarioDisponibleDTO

| Nombre | Tipo |
|---|---|
| horarioId | int |
| diaSemana | string |
| horaInicio | string |
| horaFin | string |
| cupoMaximo | int |
| cupoDisponible | int |

#### BeneficioDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| id | int | |
| titulo | string | |
| descripcion | string | |
| tipo | string | |
| porcentajeDescuento | float | |
| vigenteDesde | fecha | |
| vigenteHasta | fecha | |
| vigente | bool | Si hoy está dentro del período |

#### GimnasioConBeneficiosDTO

| Nombre | Tipo |
|---|---|
| gimnasioId | int |
| nombre | string |
| direccion | string |
| telefono | string |
| beneficios | lista de BeneficioDTO |

#### ReservaDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| id | int | |
| fechaTurno | fecha y hora | Día y hora de inicio del turno |
| horaFin | string | |
| estado | string | `Confirmada` o `Cancelada` |
| fechaSolicitud | fecha y hora | |
| fechaCancelacion | fecha y hora | null si no se canceló |
| horarioId | int | |
| gimnasioId | int | |
| nombreGimnasio | string | |

#### ReservaGimnasioDTO

| Nombre | Tipo |
|---|---|
| id | int |
| fechaTurno | fecha y hora |
| horaFin | string |
| estado | string |
| fechaCancelacion | fecha y hora |
| horarioId | int |
| nombreUsuario | string |

#### EstadisticasGimnasioDTO

| Nombre | Tipo | Descripción |
|---|---|---|
| desde | fecha | |
| hasta | fecha | |
| totalReservas | int | |
| confirmadas | int | |
| canceladas | int | |
| porcentajeCancelacion | double | |
| ocupacionPorHorario | array | Uno por horario |
| - horarioId | int | |
| - diaSemana | string | |
| - horaInicio | string | |
| - horaFin | string | |
| - cupoMaximo | int | |
| - cantidadTurnos | int | Cuántas veces cae ese día en el período |
| - reservasConfirmadas | int | |
| - cancelaciones | int | |
| - porcentajeOcupacion | double | |
