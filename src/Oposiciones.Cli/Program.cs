using Microsoft.Extensions.Logging.Abstractions;
using Oposiciones.Cli;
using Oposiciones.Domain.Catalog;
using Oposiciones.Infrastructure.Configuration;
using Oposiciones.Infrastructure.Content;

// ------------------------------------------------------------------------------------------------
// Herramienta de linea de comandos del banco de preguntas.
//
// Cubre el trabajo diario de mantener el contenido: validar los ficheros antes de subirlos, ver que
// temas siguen sin material y volcar todo el contenido a SQL Server como guion idempotente.
// ------------------------------------------------------------------------------------------------

string command = args.Length > 0 ? args[0].ToLowerInvariant() : "help";

if (command is "help" or "--help" or "-h")
{
    PrintHelp();
    return 0;
}

string contentRoot = GetOption(args, "--content") ?? FindContentRoot() ?? "content";
var options = new ContentOptions { RootPath = contentRoot, FailOnInvalidContent = false };
var loader = new ContentLoader(NullLogger<ContentLoader>.Instance);

ContentCatalog catalog;
try
{
    catalog = loader.Load(options, Directory.GetCurrentDirectory());
}
catch (Exception ex)
{
    Console.Error.WriteLine($"No se ha podido cargar el contenido: {ex.Message}");
    return 2;
}

switch (command)
{
    case "validate":
        return Validate(catalog);

    case "coverage":
        Coverage(catalog, GetOption(args, "--exam") ?? "TAI");
        return 0;

    case "sql":
        return GenerateSql(catalog, GetOption(args, "--out") ?? Path.Combine("db", "seed", "content.sql"));

    default:
        Console.Error.WriteLine($"Orden desconocida: '{command}'.");
        PrintHelp();
        return 64;
}

// ------------------------------------------------------------------------------------------------

static void PrintHelp()
{
    Console.WriteLine("""
        Sistema de Oposiciones - herramienta de contenido

        Uso: oposiciones <orden> [opciones]

        Ordenes:
          validate    Valida los ficheros de temario y banco de preguntas. Devuelve codigo de salida
                      distinto de cero si encuentra errores, por lo que sirve tal cual en un pipeline.
          coverage    Muestra cuantas preguntas hay por tema y destaca los que siguen vacios.
          sql         Genera un guion T-SQL idempotente que carga todo el contenido en SQL Server.

        Opciones:
          --content <ruta>   Carpeta de contenido (por defecto, la carpeta 'content' del repositorio).
          --exam <codigo>    Convocatoria sobre la que informar (por defecto, TAI).
          --out <fichero>    Fichero de salida de la orden 'sql'.
        """);
}

static string? GetOption(string[] args, string name)
{
    int index = Array.FindIndex(args, arg => string.Equals(arg, name, StringComparison.OrdinalIgnoreCase));
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

/// <summary>Localiza la carpeta 'content' subiendo desde el directorio actual.</summary>
static string? FindContentRoot()
{
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (directory is not null)
    {
        string candidate = Path.Combine(directory.FullName, "content");
        if (Directory.Exists(candidate))
        {
            return candidate;
        }

        directory = directory.Parent;
    }

    return null;
}

static int Validate(ContentCatalog catalog)
{
    Console.WriteLine($"Convocatorias: {catalog.Exams.Count}");
    Console.WriteLine($"Temas:         {catalog.Exams.Sum(exam => exam.AllTopics().Count())}");
    Console.WriteLine($"Preguntas:     {catalog.Questions.Count}");

    if (catalog.Issues.Count == 0)
    {
        Console.WriteLine("Contenido valido: no se han detectado incidencias.");
        return 0;
    }

    Console.Error.WriteLine();
    Console.Error.WriteLine($"Se han detectado {catalog.Issues.Count} incidencias:");
    foreach (string issue in catalog.Issues)
    {
        Console.Error.WriteLine($"  - {issue}");
    }

    return 1;
}

static void Coverage(ContentCatalog catalog, string examCode)
{
    ExamProfile? exam = catalog.FindExam(examCode);
    if (exam is null)
    {
        Console.Error.WriteLine($"No existe la convocatoria '{examCode}'.");
        return;
    }

    Console.WriteLine($"Cobertura del banco - {exam.Code}");
    Console.WriteLine(new string('-', 96));

    var byTopic = catalog.Questions
        .Where(question => string.Equals(question.ExamCode, exam.Code, StringComparison.OrdinalIgnoreCase))
        .GroupBy(question => question.TopicId)
        .ToDictionary(group => group.Key, group => group.ToList());

    var empty = new List<string>();

    foreach (SyllabusBlock block in exam.Blocks.OrderBy(block => block.DisplayOrder))
    {
        Console.WriteLine();
        Console.WriteLine($"Bloque {block.Code} - {block.Name}");

        foreach (SyllabusTopic topic in block.Topics.OrderBy(topic => topic.Number))
        {
            byTopic.TryGetValue(topic.Id, out var questions);
            int count = questions?.Count ?? 0;
            double average = count == 0 ? 0 : questions!.Average(question => (int)question.Difficulty);

            string title = topic.Title.Length > 58 ? topic.Title[..55] + "..." : topic.Title;
            string bar = new('#', Math.Min(count, 30));
            Console.WriteLine($"  T{topic.Number,-3} {title,-60} {count,3}  dif.{average:0.0} {bar}");

            if (count == 0)
            {
                empty.Add($"{block.Code}/T{topic.Number}");
            }
        }
    }

    Console.WriteLine();
    Console.WriteLine($"Total: {byTopic.Values.Sum(list => list.Count)} preguntas en {exam.AllTopics().Count()} temas.");
    Console.WriteLine(empty.Count == 0
        ? "Todos los temas tienen al menos una pregunta."
        : $"Temas sin preguntas ({empty.Count}): {string.Join(", ", empty)}");
}

static int GenerateSql(ContentCatalog catalog, string outputPath)
{
    string? directory = Path.GetDirectoryName(outputPath);
    if (!string.IsNullOrEmpty(directory))
    {
        Directory.CreateDirectory(directory);
    }

    string script = SqlSeedWriter.Build(catalog);
    File.WriteAllText(outputPath, script);

    Console.WriteLine($"Guion generado en '{outputPath}'.");
    Console.WriteLine($"Incluye {catalog.Exams.Count} convocatorias, "
        + $"{catalog.Exams.Sum(exam => exam.AllTopics().Count())} temas y {catalog.Questions.Count} preguntas.");
    return 0;
}
