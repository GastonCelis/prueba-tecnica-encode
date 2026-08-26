# Arquitectura de la solución

Este documento describe el diseño del backend: la organización en capas, la
separación entre los roles Tenant e Issuer, el flujo de cada caso de uso y
el modelo de datos.

Las decisiones puntuales y su justificación están en
[DECISIONES.md](DECISIONES.md).

## Visión general

El sistema emite credenciales verificables para los socios del club y las
persiste firmadas. El enunciado exige separar, a nivel de servicio, dos
roles:

- **Tenant** — el club. Recibe los datos del socio, arma el
  `credentialSubject`, solicita la emisión y persiste el resultado.
- **Issuer** — el emisor de credenciales. Recibe un `credentialSubject`,
  le agrega los campos del protocolo (`id`, `type`, `issuer`, `validFrom`,
  `validUntil`, `proof`) y devuelve la credencial firmada.

El Issuer es un servicio in-process sin endpoint propio, según la sección
4.2.1 del enunciado. La frontera entre ambos roles se materializa en la
interfaz `ICredentialIssuer` y en la ubicación del código, no en un límite
de red.

## Organización en capas

El backend se divide en tres proyectos:

| Proyecto                | Responsabilidad                            | Depende de                 |
| ----------------------- | ------------------------------------------ | -------------------------- |
| `Wallet.Api`            | Controller, DTOs, configuración y arranque | `Domain`, `Infrastructure` |
| `Wallet.Domain`         | Entidades, contratos y casos de uso        | —                          |
| `Wallet.Infrastructure` | EF Core, SQLite y el servicio Issuer       | `Domain`                   |

`Wallet.Domain` no referencia ningún otro proyecto. Define las entidades,
los casos de uso y los **contratos** de todo lo que necesita del exterior:

| Contrato                | Implementación         | Ubicación                    |
| ----------------------- | ---------------------- | ---------------------------- |
| `ICredentialIssuer`     | `HmacCredentialIssuer` | `Infrastructure/Issuing`     |
| `ICredentialRepository` | `CredentialRepository` | `Infrastructure/Persistence` |
| `IUnitOfWork`           | `UnitOfWork`           | `Infrastructure/Persistence` |

Las dependencias apuntan hacia el dominio. Esto permite reemplazar la firma
mock por un esquema real, o SQLite por otro motor, cambiando únicamente el
registro correspondiente en `Program.cs`.

## Componentes

### Wallet.Api

Expone un único controller, `CredentialsController`, con dos endpoints.
Sus responsabilidades son:

- Recibir y validar la entrada mediante DTOs con `DataAnnotations`
- Traducir la categoría de su representación textual al enum del dominio
- Invocar el caso de uso correspondiente
- Traducir el resultado o la excepción a un código HTTP

No contiene lógica de negocio.

### Wallet.Domain

| Elemento                                             | Rol                                                       |
| ---------------------------------------------------- | --------------------------------------------------------- |
| `Socio`, `Credential`                                | Entidades persistidas                                     |
| `CredentialSubject`, `Proof`, `VerifiableCredential` | Modelo de la credencial, definido como `record` inmutable |
| `EmitirCredencialService`                            | Caso de uso UC01 (rol Tenant)                             |
| `ListarCredencialesService`                          | Caso de uso UC02 (rol Tenant)                             |

El modelo de la credencial se define con `record` porque una credencial
firmada no debe poder mutar: si un campo cambiara después de la emisión, la
firma dejaría de corresponder al contenido.

### Wallet.Infrastructure

**Issuing** — el servicio Issuer y todo lo relativo a la firma:

| Archivo                   | Responsabilidad                                   |
| ------------------------- | ------------------------------------------------- |
| `HmacCredentialIssuer`    | Genera los campos del protocolo y firma           |
| `CanonicalCredential`     | Define el orden alfabético de las claves          |
| `CredentialCanonicalizer` | Produce el JSON canónico exacto                   |
| `VcSerializer`            | Serializa la credencial completa para persistirla |
| `IssuerOptions`           | Configuración leída del entorno                   |

**Persistence** — el acceso a datos: `WalletDbContext`, las
configuraciones de cada entidad, las migraciones, el repositorio y la
unidad de trabajo.

## Flujo de UC01 — Alta de credencial

1. El cliente envía `POST /api/credentials` con los datos del socio.
2. `CredentialsController` valida el DTO y traduce la categoría al enum
   del dominio.
3. `EmitirCredencialService` abre una transacción.
4. Se inserta el socio mediante `ICredentialRepository.AgregarSocioAsync`.
   SQLite asigna el `Id` autoincremental, del que deriva `numeroSocio`.
5. Se arma el `CredentialSubject` con los datos del socio.
6. `ICredentialIssuer.Emitir` genera `id`, `type`, `issuer`, `validFrom`
   y `validUntil`, canonicaliza la credencial y calcula el HMAC-SHA256.
7. Se persiste la credencial completa serializada mediante
   `ICredentialRepository.AgregarCredencialAsync`.
8. Se confirma la transacción y se responde `201 Created` con el número
   de socio y la vigencia.

### Atomicidad

El orden de las operaciones responde a una restricción del modelo:
`numeroSocio` forma parte del JSON que se firma, por lo que debe obtenerse
**antes** de invocar al Issuer. Pero la extensión 5a del enunciado exige
que, si la firma falla, no se persista nada.

La solución es envolver toda la operación en una única transacción. Si
`ICredentialIssuer.Emitir` lanza una excepción, la transacción se libera
sin commit y Entity Framework Core revierte también el alta del socio. No
quedan registros huérfanos ni se consume un número de socio.

### Manejo de errores

| Situación         | Respuesta                                      |
| ----------------- | ---------------------------------------------- |
| Entrada inválida  | `400` con el detalle por campo                 |
| DNI ya registrado | `409`                                          |
| Falla la firma    | `500` con `ProblemDetails`, sin persistir nada |

## Flujo de UC02 — Listado de credenciales

1. El cliente envía `GET /api/credentials` con los filtros opcionales
   `busqueda`, `categoria` y `estado`.
2. `CredentialsController` valida los parámetros y traduce la categoría al
   enum del dominio.
3. `ListarCredencialesService` invoca a
   `ICredentialRepository.ListarAsync`, que aplica los filtros de búsqueda
   y estado en la consulta a la base y ordena por vigencia en memoria.
4. El caso de uso aplica el filtro por categoría en memoria.
5. `CredencialListadoDto.Desde` proyecta cada credencial a la vista del
   listado.
6. Se responde `200 OK` con el array resultante.

Los datos que se muestran se extraen del JSON persistido, no de la tabla
`socios`. La credencial es un documento histórico: refleja lo que se firmó
al momento de la emisión, no el estado actual del socio.

Si no hay credenciales que coincidan, la respuesta es `200` con un array
vacío. Un `404` sería incorrecto: el recurso existe, está vacío.

## Modelo de datos

Dos tablas, relacionadas uno a muchos: un socio puede tener varias
credenciales emitidas.

### socios

| Columna     | Tipo    | Notas                              |
| ----------- | ------- | ---------------------------------- |
| `Id`        | INTEGER | Clave primaria, autoincremental    |
| `Did`       | TEXT    | Índice único. `did:example:{Guid}` |
| `Nombre`    | TEXT    |                                    |
| `Apellido`  | TEXT    |                                    |
| `Dni`       | TEXT    | Índice único                       |
| `Categoria` | TEXT    | `Adulto`, `Juvenil` o `Nino`       |
| `FotoUrl`   | TEXT    |                                    |

Existe como entidad propia porque el enunciado establece que
`credentialSubject.id` "se genera una vez y se persiste", y que
`numeroSocio` es secuencial y consistente entre reinicios. Ambos son
atributos de la persona, no del documento emitido.

`numeroSocio` no es una columna: es una propiedad derivada del `Id`
autoincremental, formateada a seis dígitos. Delegar la secuencia al motor
garantiza unicidad sin lógica propia de concurrencia.

### credentials

| Columna      | Tipo    | Notas                                        |
| ------------ | ------- | -------------------------------------------- |
| `Id`         | TEXT    | Clave primaria. Guid generado por el Issuer  |
| `SocioId`    | INTEGER | Clave foránea a `socios`, con `Restrict`     |
| `VcJson`     | TEXT    | La credencial completa, tal como fue firmada |
| `ValidFrom`  | TEXT    | Columna de proyección                        |
| `ValidUntil` | TEXT    | Columna de proyección                        |
| `Status`     | INTEGER | Columna de proyección                        |

`VcJson` contiene la credencial verificable completa. Las tres columnas de
proyección duplican información presente en ese JSON y existen únicamente
para consultar y ordenar sin deserializar.

Reconstruir la credencial a partir de columnas sueltas arriesgaría alterar
el orden de las claves o el formato de las fechas, rompiendo la
correspondencia con la firma.

La clave foránea usa `Restrict`: no se puede eliminar un socio que tenga
credenciales emitidas.

## Canonicalización y firma

La sección 4.1.2 del enunciado fija un formato canónico exacto como entrada
del HMAC. El sistema lo implementa así:

| Regla del enunciado                                  | Implementación                                  |
| ---------------------------------------------------- | ----------------------------------------------- |
| JSON compacto                                        | `WriteIndented = false`                         |
| Claves de primer nivel en orden alfabético           | `JsonPropertyOrder` en `CanonicalCredential`    |
| Subcampos de `credentialSubject` en orden alfabético | `JsonPropertyOrder` en `CanonicalSubject`       |
| ISO-8601 UTC con precisión de segundos y sufijo `Z`  | Formato explícito y truncado por Unix timestamp |
| Caracteres no ASCII sin escapar                      | `JavaScriptEncoder.UnsafeRelaxedJsonEscaping`   |

El resultado se firma con `HMAC-SHA256` usando la clave leída del entorno,
y el hash se codifica en base64.

Existen **dos serializaciones distintas** de la credencial y no deben
confundirse:

- La **canónica** (`CredentialCanonicalizer`) excluye `proof`, ordena
  alfabéticamente, y existe solo para calcular el hash.
- La **completa** (`VcSerializer`) incluye `proof`, sigue el orden de la
  tabla del enunciado, y es la que se persiste y se devuelve.

`validFrom` se calcula una sola vez por emisión y se reutiliza para
`validUntil` y para `proof.created`, de modo que la coincidencia entre
ambos campos queda garantizada por construcción.

## Configuración

Toda la configuración se maneja por variables de entorno, cargadas desde un
archivo `.env` mediante `DotNetEnv`. La carga ocurre como primera
instrucción de `Program.cs`, antes de construir el host, para que las
variables existan en el proceso cuando ASP.NET Core arma su configuración.

La clave de firma no tiene valor por defecto: si falta, el Issuer lanza una
excepción y la emisión falla sin persistir nada.

## Alcance no implementado

Fuera de alcance:

- Verificación de credenciales
- Revocación o cambio de estado — `credentialStatus` se persiste pero siempre vale `0` (activa)
- Autenticación y autorización
- Fidelidad estricta a la especificación W3C VC/DID
- Paginación del listado

## Frontend

Aplicación Angular independiente que consume la API por HTTP. No comparte
código con el backend: la comunicación es exclusivamente a través de los
dos endpoints REST.

### Organización

| Carpeta          | Contenido                                             |
| ---------------- | ----------------------------------------------------- |
| `models/`        | Interfaces TypeScript que espejan los DTOs de la API  |
| `services/`      | `CredencialesService`, único punto de acceso a la API |
| `pages/listado/` | Pantalla de listado (UC02)                            |
| `pages/alta/`    | Pantalla de alta y resultado (UC01)                   |

Ambas pantallas se cargan de forma diferida mediante `loadComponent`: el
código de cada una se descarga al navegar a su ruta.

### Estado y comunicación

El estado local de cada pantalla se maneja con _signals_. Toda llamada a la
API pasa por `CredencialesService`, que centraliza la URL base y la
construcción de los parámetros de consulta.

Los errores del backend llegan en formato `ProblemDetails` y se muestran al
usuario leyendo el campo `detail`, de modo que un DNI duplicado o un fallo
en la firma se informan con el mensaje real del servidor.

### Validación

El formulario de alta usa _Reactive Forms_ con las mismas reglas que valida
el backend: campos obligatorios, DNI de 7 a 9 dígitos, categoría dentro del
conjunto permitido y foto con formato de URL.

La validación del cliente evita peticiones innecesarias, pero no reemplaza
la del servidor: ambas se mantienen.

### CORS

El frontend corre en `localhost:4200` y la API en `localhost:7240`. La API
declara una política de CORS que autoriza ese origen.
