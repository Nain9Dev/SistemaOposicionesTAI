# Contenido

Temarios oficiales y banco de preguntas. Estos ficheros son la **fuente única de verdad**: los lee
el proveedor en memoria de la API y de ellos se genera el guion de carga a SQL Server, de modo que
ambos caminos parten exactamente del mismo material.

```
content/
├── exams/       Un fichero por convocatoria, con su temario, su baremo y el formato del ejercicio.
└── questions/   Banco de preguntas, repartido en ficheros por bloque.
```

## Añadir preguntas

Cada pregunta se escribe así:

```json
{
  "id": "TAI-B1-T01-011",
  "blockCode": "I",
  "topicNumber": 1,
  "difficulty": 3,
  "statement": "¿Qué establece el artículo 9.3 de la Constitución Española?",
  "options": ["Primera opción", "Segunda opción", "Tercera opción", "Cuarta opción"],
  "correctIndex": 2,
  "explanation": "Justificación de la respuesta, visible en modo estudio.",
  "source": {
    "reference": "Constitución Española, artículo 9.3",
    "publication": "BOE núm. 311, de 29 de diciembre de 1978",
    "url": "https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229"
  },
  "tags": ["constitucion", "titulo-preliminar"]
}
```

Reglas que se comprueban automáticamente:

- `id` es único en todo el banco y estable en el tiempo. Es la clave por la que se reimporta el
  contenido sin duplicar preguntas ni romper el historial de intentos ya registrados.
- `blockCode` y `topicNumber` tienen que existir en el temario de la convocatoria.
- `correctIndex` es de base cero y debe apuntar a una opción real.
- No puede haber dos opciones con el mismo texto.
- `difficulty` va de 1 (básica) a 5 (muy difícil).
- Toda pregunta debe citar su `source` y aportar `explanation`: la fuente oficial es el criterio con
  el que se revisa y se defiende cada respuesta.

## Antes de subir cambios

```bash
# Valida el contenido: devuelve código de salida distinto de cero si algo está mal
dotnet run --project src/Oposiciones.Cli -- validate

# Muestra cuántas preguntas hay por tema y cuáles siguen vacíos
dotnet run --project src/Oposiciones.Cli -- coverage

# Regenera el guion de carga a SQL Server (obligatorio si has tocado el contenido)
dotnet run --project src/Oposiciones.Cli -- sql --out db/seed/content.sql
```

La integración continua ejecuta estas tres órdenes y falla si el contenido es inválido o si el
guion SQL se ha quedado desincronizado.

## Añadir otra oposición

Crea un fichero nuevo en `exams/` con su código, su temario, su baremo y el formato de su ejercicio,
y otro en `questions/` con `examCode` apuntando a él. No hay que tocar código: la API expondrá la
convocatoria nueva en `/api/exams` y podrá generar tests de ella con su propio baremo.
