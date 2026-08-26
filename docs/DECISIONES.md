# Decisiones de diseño

Registro de las decisiones tomadas.

## D1 — Motor de persistencia: SQLite

**Decisión:** SQLite mediante Entity Framework Core.

**Motivo:** SQLite es un motor embebido basado en archivo: no requiere un servicio propio ni docker-compose, lo que simplifica el levantado del entorno por parte del evaluador.

## D2 — Estructura de capas

**Decisión:** tres proyectos — `Api`, `Domain`, `Infrastructure`.

**Motivo:** Tres proyectos alcanzan para que la separación entre el rol Tenant y el rol Issuer sea visible en la estructura y no solo en la documentación. `Domain` no depende de nadie y define el contrato `ICredentialIssuer`; `Infrastructure` lo implementa. El controller depende de la interfaz, por lo que reemplazar la firma mock por una firma real (JWS) no impacta en la capa de aplicación.

## D3 — Separación Tenant / Issuer

**Decisión:** el Issuer es un servicio in-process, sin endpoint HTTP propio, expuesto tras la interfaz `ICredentialIssuer`.

**Motivo:** es lo que indica el enunciado (sección 4.2.1).

## D4 — Entidad Socio separada de Credential

**Decisión:** se persisten dos entidades, `Socios` y `Credentials`.

**Motivo:** el enunciado establece que `credentialSubject.id` (el DID del socio) "se genera una vez y se persiste", y que `numeroSocio` es secuencial y consistente entre reinicios. Ambos son atributos de la persona, no del documento emitido. Modelarlos en una entidad `Socio` permite que un mismo socio reciba futuras credenciales conservando su identidad.

## D5 — Generación de numeroSocio

**Decisión:** columna de identidad autoincremental en `Socios`, formateada a 6 dígitos con ceros a la izquierda (ej. `000123`).

**Motivo:** delegar la secuencia al motor garantiza unicidad y consistencia entre reinicios sin lógica propia de concurrencia.

**Consideración crítica:** `numeroSocio` forma parte del JSON que se firma, por lo que debe obtenerse antes de invocar al Issuer. Para cumplir la extensión 5a (si falla la firma no se persiste nada), toda la operación de alta se ejecuta dentro de una única transacción: alta del socio, firma y alta de la credencial. Un fallo en la firma provoca rollback y no deja registros huérfanos.

## D6 — Persistencia de la credencial como documento JSON

**Decisión:** la VC completa se guarda serializada en una columna de texto, con columnas adicionales (`ValidFrom`, `ValidUntil`, `Status`, `SocioId`) para consulta.

**Motivo:** el enunciado exige persistir la credencial completa. Reconstruirla desde columnas sueltas arriesga alterar el orden de claves o el formato de fechas, rompiendo la correspondencia con la firma. Se conserva el JSON exacto que fue firmado y las columnas de proyección permiten listar y filtrar sin deserializar.

## D7 — Codificación de proofValue

**Decisión:** base64.

**Motivo:** el enunciado indica explícitamente base64 en la regla del campo. Se observa que el ejemplo ilustrativo de `proof` muestra un valor de 64 caracteres hexadecimales, inconsistente con esa regla. Se implementa la regla escrita y se deja constancia de la observación.

## D8 — Foto del socio

**Decisión:** solo URL.

**Motivo:** el enunciado define la URL como requisito mínimo y el upload de archivo como opcional. Se prioriza completar el flujo principal y la documentación.

## D9 — Dockerización

**Decisión:** No Dockerizar.

**Motivo:** con SQLite no hay servicio externo que contenerizar, y el enunciado marca la dockerización de backend y frontend como opcional.

## D11 — Configuración por archivo .env

**Decisión:** la configuración se maneja por variables de entorno, cargadas desde un archivo `.env` mediante el paquete `DotNetEnv`.

## D12 — Ordenamiento del listado en memoria

**Decisión:** el listado de credenciales se ordena en memoria y no en la consulta SQL.

**Motivo:** SQLite no tiene un tipo de dato nativo para fechas y almacena `DateTimeOffset` como texto, por lo que el proveedor de EF Core no admite `ORDER BY` sobre esas columnas. El ordenamiento se resuelve con LINQ to Objects tras materializar la consulta.

## D13 — Filtros del listado

**Decisión:** se implementan tres filtros opcionales sobre el listado: búsqueda parcial por nombre, apellido o DNI; categoría; y estado de la credencial.

**Motivo:** se incluyen por su bajo costo y porque cubren los casos de uso reales de una pantalla de listado.

## D14 — Tecnología del frontend

**Decisión:** Angular con componentes standalone, signals y Reactive Forms.

**Motivo:** se utiliza Angular por ser el stack utilizado en el equipo.

## D15 — Estructura del repositorio

**Decisión:** un único repositorio con el backend y el frontend.

**Motivo:** mantener ambas aplicaciones juntas permite un único `README.md` con las instrucciones de levantado y un historial de commits unificado, sin necesidad de correlacionar dos repositorios separados.

## D16 — Diseño visual

**Decisión:** estilos propios, sin librería de componentes.

**Motivo:** el enunciado admite explícitamente un estilo simple y minimalista.

## D17 — Alta en modal sobre el listado

**Decisión:** el alta de credencial se resuelve en un modal sobre la pantalla de listado, en lugar de una ruta y una pantalla propias.

**Motivo:** el enunciado requiere que la interfaz implemente las funcionalidades de alta y listado, y sugiere mostrar una pantalla de resultado con el número de socio asignado antes de volver al listado.

Mantener el listado visible de fondo evita perder el contexto y permite refrescarlo automáticamente al cerrar, sin una navegación intermedia.
