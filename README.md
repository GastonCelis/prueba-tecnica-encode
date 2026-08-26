# Wallet de Credenciales Verificables

Sistema de emisión y gestión de credenciales digitales verificables para
socios del Club de Fútbol de Córdoba, con firma criptográfica (HMAC-SHA256).

## Stack

| Backend - .NET 10 (ASP.NET Core Web API) |
| Persistencia - SQLite + Entity Framework Core |
| Firma - HMAC-SHA256 (firma mock, según enunciado) |
| Documentación de API - OpenAPI + Swagger UI |

## Requisitos previos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- [Node.js 20](https://nodejs.org/)

Verificar la instalación:

```bash
dotnet --list-sdks
node -v
```

## Cómo levantar el proyecto

Con la API corriendo, abrir `/swagger` (por ejemplo `https://localhost:7240/swagger`) para explorar y ejecutar los endpoints.

## Variables de entorno

Toda la configuración se maneja por variables de entorno, cargadas desde el archivo `.env` mediante el paquete `DotNetEnv`. Los valores no sensibles tienen un valor por defecto en `appsettings.json`; la clave de firma no lo tiene y es obligatoria.

| Variable                     | Descripción                                               | Valor por defecto                    |
| ---------------------------- | --------------------------------------------------------- | ------------------------------------ |
| `Issuer__SigningKey`         | Clave secreta del HMAC-SHA256. **Obligatoria.**           | —                                    |
| `Issuer__Did`                | Identidad digital del emisor                              | `did:example:futbol`                 |
| `Issuer__BaseUrl`            | Dominio bajo el que se construye el `id` de la credencial | `https://credenciales.futbol.com.ar` |
| `Issuer__VerificationMethod` | Referencia a la clave usada para firmar                   | `did:example:futbol#key-1`           |
| `ConnectionStrings__Default` | Cadena de conexión a SQLite                               | `Data Source=wallet.db`              |

El doble guión bajo representa el anidamiento de la configuración y sobrescribe lo definido en `appsettings.json`.

`Issuer__Did`, `Issuer__BaseUrl` e `Issuer__VerificationMethod` son identificadores definidos por el enunciado (sección 4.1.2), no direcciones accesibles: el sistema no realiza peticiones HTTP contra ellas.

Generar clave de firma:

```bash
# Linux / macOS
openssl rand -base64 32
```

```powershell
# Windows (PowerShell)
$bytes = New-Object byte[] 32
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

## API

| Método | Ruta               | Descripción                             |
| ------ | ------------------ | --------------------------------------- |
| `POST` | `/api/credentials` | Alta de credencial de socio (UC01)      |
| `GET`  | `/api/credentials` | Listado de credenciales emitidas (UC02) |

### Alta de credencial

```bash
curl -X POST https://localhost:7240/api/credentials \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Juan",
    "apellido": "Pérez",
    "dni": "30123456",
    "categoria": "adulto",
    "foto": "https://cdn.futbol.com.ar/socios/8f14e45f.jpg"
  }'
```

Campos aceptados:

| Campo       | Regla                                         |
| ----------- | --------------------------------------------- |
| `nombre`    | Requerido, hasta 100 caracteres               |
| `apellido`  | Requerido, hasta 100 caracteres               |
| `dni`       | Requerido, entre 7 y 9 dígitos                |
| `categoria` | Requerido, uno de `adulto`, `juvenil`, `niño` |
| `foto`      | Requerido, URL válida                         |

El resto de los campos de la credencial —el `id` de la VC, el DID del
sujeto, el número de socio, la vigencia, el estado y la firma— los genera
el sistema y no son editables.

Respuestas:

| Código | Situación                                                              |
| ------ | ---------------------------------------------------------------------- |
| `201`  | Credencial emitida. Devuelve el `id`, el número de socio y la vigencia |
| `400`  | Datos de entrada inválidos, con el detalle por campo                   |
| `409`  | Ya existe un socio registrado con ese DNI                              |
| `500`  | Falló la emisión en el Issuer. No se persiste nada                     |

### Listado de credenciales

```bash
curl "https://localhost:7240/api/credentials?busqueda=perez&categoria=adulto"
```

Parámetros de consulta, todos opcionales:

| Parámetro   | Descripción                                    |
| ----------- | ---------------------------------------------- |
| `busqueda`  | Coincidencia parcial en nombre, apellido o DNI |
| `categoria` | `adulto`, `juvenil` o `niño`                   |
| `estado`    | `0` activa, `1` revocada, `2` suspendida       |

Devuelve `200` con un array de credenciales. Si no hay ninguna que
coincida, devuelve `200` con un array vacío.

Cada elemento incluye los datos del socio, la vigencia y el estado, más un
campo `vc` con la credencial verificable completa —incluidos los campos
técnicos del protocolo (`id`, `type`, `issuer`, `proof`)— para su
presentación en un detalle expandible.

## Documentación

- [Arquitectura](docs/ARQUITECTURA.md)
- [Decisiones de diseño](docs/DECISIONES.md)
