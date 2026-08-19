# Base de datos

Esquema T-SQL del Sistema de Oposiciones. El diseño es multi-convocatoria desde la raíz: añadir una
oposición nueva es insertar filas en `dbo.Exams` y su temario, nunca modificar tablas.

## Estructura

```
db/
├── migrations/   Guiones de esquema y procedimientos almacenados. Se ejecutan en orden.
└── seed/         Contenido generado automáticamente desde content/. No se edita a mano.
```

## Aplicar las migraciones

Ejecuta los guiones de `migrations/` en orden numérico sobre la base de datos. Todos son
idempotentes: pueden volver a ejecutarse sobre una base ya creada sin efectos secundarios.

```bash
sqlcmd -S localhost -U sa -P "<contraseña>" -Q "IF DB_ID('OposicionesTAI') IS NULL CREATE DATABASE OposicionesTAI;"

for script in db/migrations/*.sql; do
  sqlcmd -S localhost -U sa -P "<contraseña>" -d OposicionesTAI -b -i "$script"
done
```

| Guion | Contenido |
| :--- | :--- |
| `001_schema.sql` | Tablas, claves, restricciones e índices. |
| `002_types.sql` | Tipos de tabla definidos por el usuario, usados para enviar conjuntos completos en una sola llamada. |
| `003_procedures_catalog.sql` | Lectura de convocatorias, bloques y temas. |
| `004_procedures_questions.sql` | Búsqueda, extracción reproducible y cobertura del banco. |
| `005_procedures_tests.sql` | Creación y lectura de tests generados. |
| `006_procedures_attempts.sql` | Ciclo de vida de los intentos y analítica por tema. |
| `007_procedures_import.sql` | Altas y actualizaciones idempotentes de contenido. |

## Cargar el contenido

`db/seed/content.sql` se genera a partir de los ficheros JSON de `content/` y solo invoca los
procedimientos de importación, que trabajan por clave de negocio. Puede ejecutarse tantas veces
como haga falta: la segunda ejecución actualiza lo que cambió y deja intacto lo demás, sin duplicar
preguntas ni romper los intentos ya registrados.

```bash
# Regenerar el guion tras editar content/
dotnet run --project src/Oposiciones.Cli -- sql --out db/seed/content.sql

# Cargarlo
sqlcmd -S localhost -U sa -P "<contraseña>" -d OposicionesTAI -b -i db/seed/content.sql
```

## Decisiones de diseño

**El baremo se copia sobre cada test al crearlo.** Un examen realizado hace meses debe seguir
corrigiéndose con las reglas vigentes cuando se generó, aunque después cambien las bases de la
convocatoria.

**La nota no se calcula en la base de datos.** Los procedimientos guardan respuestas y resultados;
la corrección vive en el dominio .NET, donde el baremo es un dato configurable. Cambiar la
penalización no obliga a reescribir ningún procedimiento almacenado.

**La extracción aleatoria es reproducible.** `dbo.QuestionsDraw` no ordena por `NEWID()`, sino por
un hash de (identificador de pregunta, semilla). Con la misma semilla y el mismo banco la
extracción es idéntica, que es lo que permite compartir un examen o repetirlo tal cual.

**Las opciones se sincronizan por posición, no se borran y reinsertan.** Sus identificadores ya
pueden estar referenciados por tests generados e intentos respondidos; perderlos reescribiría el
historial del opositor. Solo se eliminan las opciones sobrantes que nadie referencia.

**Las etiquetas se normalizan en su propia tabla.** Permite filtrar por etiqueta con un índice en
lugar de con un `LIKE` sobre toda la tabla de preguntas.
