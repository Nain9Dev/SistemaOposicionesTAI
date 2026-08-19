using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Scoring;
using Oposiciones.Infrastructure.Configuration;

namespace Oposiciones.Infrastructure.Content;

/// <summary>
/// Lee la carpeta de contenido, valida cada fichero y construye el catalogo del dominio.
/// <para>
/// Los identificadores numericos se asignan de forma determinista a partir del orden natural del
/// contenido (convocatoria, bloque, tema, identificador externo). Dos arranques con el mismo
/// contenido producen exactamente los mismos identificadores, que es lo que permite que el
/// proveedor en memoria sea utilizable de verdad y no solo una maqueta.
/// </para>
/// </summary>
public sealed class ContentLoader
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private readonly ILogger<ContentLoader> _logger;

    public ContentLoader(ILogger<ContentLoader> logger)
    {
        _logger = logger;
    }

    /// <summary>Opciones de serializacion usadas para leer y escribir contenido.</summary>
    public static JsonSerializerOptions JsonOptions => SerializerOptions;

    public ContentCatalog Load(ContentOptions options, string basePath)
    {
        ArgumentNullException.ThrowIfNull(options);

        string root = Path.IsPathRooted(options.RootPath)
            ? options.RootPath
            : Path.Combine(basePath, options.RootPath);

        if (!Directory.Exists(root))
        {
            _logger.LogWarning("No se ha encontrado la carpeta de contenido '{Root}'. El catalogo queda vacio.", root);
            return ContentCatalog.Empty;
        }

        var issues = new List<string>();
        IReadOnlyList<ExamProfile> exams = LoadExams(
            Path.Combine(root, options.ExamsFolder),
            issues,
            out Dictionary<string, SyllabusTopic> topicIndex);

        IReadOnlyList<Question> questions = LoadQuestions(
            Path.Combine(root, options.QuestionsFolder),
            topicIndex,
            issues);

        if (issues.Count > 0)
        {
            foreach (string issue in issues)
            {
                _logger.LogError("Contenido invalido: {Issue}", issue);
            }

            if (options.FailOnInvalidContent)
            {
                var message = new StringBuilder("El contenido cargado contiene errores:");
                foreach (string issue in issues.Take(20))
                {
                    message.Append(CultureInfo.InvariantCulture, $"{Environment.NewLine} - {issue}");
                }

                if (issues.Count > 20)
                {
                    message.Append(CultureInfo.InvariantCulture, $"{Environment.NewLine} - (+{issues.Count - 20} mas)");
                }

                throw new InvalidOperationException(message.ToString());
            }
        }

        _logger.LogInformation(
            "Contenido cargado desde '{Root}': {Exams} convocatorias, {Topics} temas y {Questions} preguntas.",
            root,
            exams.Count,
            topicIndex.Count,
            questions.Count);

        return new ContentCatalog(exams, questions, issues);
    }

    private IReadOnlyList<ExamProfile> LoadExams(
        string folder,
        List<string> issues,
        out Dictionary<string, SyllabusTopic> topicIndex)
    {
        topicIndex = new Dictionary<string, SyllabusTopic>(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(folder))
        {
            issues.Add($"No existe la carpeta de convocatorias '{folder}'.");
            return Array.Empty<ExamProfile>();
        }

        var files = Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();

        var parsed = new List<ExamFile>(files.Count);
        foreach (string file in files)
        {
            ExamFile? exam = Deserialize<ExamFile>(file, issues);
            if (exam is null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(exam.Code))
            {
                issues.Add($"{Path.GetFileName(file)}: la convocatoria no declara 'code'.");
                continue;
            }

            parsed.Add(exam);
        }

        var result = new List<ExamProfile>(parsed.Count);
        int examId = 0;
        int blockId = 0;
        int topicId = 0;

        foreach (ExamFile file in parsed.OrderBy(exam => exam.Code, StringComparer.OrdinalIgnoreCase))
        {
            examId++;
            string examCode = file.Code.Trim().ToUpperInvariant();
            var blocks = new List<SyllabusBlock>(file.Blocks.Count);

            IEnumerable<BlockFile> orderedBlocks = file.Blocks
                .OrderBy(block => block.DisplayOrder)
                .ThenBy(block => block.Code, StringComparer.Ordinal);

            foreach (BlockFile block in orderedBlocks)
            {
                blockId++;
                string blockCode = block.Code.Trim();
                var topics = new List<SyllabusTopic>(block.Topics.Count);

                foreach (TopicFile topic in block.Topics.OrderBy(topic => topic.Number))
                {
                    topicId++;
                    var domainTopic = new SyllabusTopic
                    {
                        Id = topicId,
                        BlockId = blockId,
                        BlockCode = blockCode,
                        ExamCode = examCode,
                        Number = topic.Number,
                        Title = topic.Title.Trim(),
                        Slug = string.IsNullOrWhiteSpace(topic.Slug)
                            ? Slugify(topic.Title)
                            : topic.Slug.Trim(),
                        Keywords = topic.Keywords.Where(k => !string.IsNullOrWhiteSpace(k)).ToList(),
                    };

                    topics.Add(domainTopic);

                    string key = TopicKey(examCode, blockCode, topic.Number);
                    if (!topicIndex.TryAdd(key, domainTopic))
                    {
                        issues.Add($"Tema duplicado en el temario: {key}.");
                    }
                }

                blocks.Add(new SyllabusBlock
                {
                    Id = blockId,
                    ExamId = examId,
                    ExamCode = examCode,
                    Code = blockCode,
                    Name = block.Name.Trim(),
                    DisplayOrder = block.DisplayOrder,
                    ExamWeightPercent = block.ExamWeightPercent,
                    Topics = topics,
                });
            }

            result.Add(new ExamProfile
            {
                Id = examId,
                Code = examCode,
                Name = file.Name.Trim(),
                Authority = file.Authority.Trim(),
                Description = file.Description.Trim(),
                OfficialSource = ToSource(file.Source),
                Scoring = ToScoring(file.Scoring),
                Format = ToFormat(file.Format),
                Blocks = blocks,
            });
        }

        return result;
    }

    private IReadOnlyList<Question> LoadQuestions(
        string folder,
        IReadOnlyDictionary<string, SyllabusTopic> topicIndex,
        List<string> issues)
    {
        if (!Directory.Exists(folder))
        {
            issues.Add($"No existe la carpeta del banco de preguntas '{folder}'.");
            return Array.Empty<Question>();
        }

        var entries = new List<(string File, string ExamCode, QuestionEntry Entry)>();

        foreach (string file in Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories)
                     .OrderBy(path => path, StringComparer.Ordinal))
        {
            QuestionFile? bank = Deserialize<QuestionFile>(file, issues);
            if (bank is null)
            {
                continue;
            }

            string examCode = bank.ExamCode.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(examCode))
            {
                issues.Add($"{Path.GetFileName(file)}: el banco no declara 'examCode'.");
                continue;
            }

            foreach (QuestionEntry entry in bank.Questions)
            {
                entries.Add((Path.GetFileName(file), examCode, entry));
            }
        }

        var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var questions = new List<Question>(entries.Count);
        long questionId = 0;

        foreach ((string file, string examCode, QuestionEntry entry) in entries
                     .OrderBy(item => item.ExamCode, StringComparer.Ordinal)
                     .ThenBy(item => item.Entry.Id, StringComparer.Ordinal))
        {
            string externalId = entry.Id.Trim();
            string where = $"{file} [{(externalId.Length == 0 ? "sin id" : externalId)}]";

            if (externalId.Length == 0)
            {
                issues.Add($"{where}: la pregunta no declara 'id'.");
                continue;
            }

            if (!seenIds.Add(externalId))
            {
                issues.Add($"{where}: identificador de pregunta duplicado.");
                continue;
            }

            string topicKey = TopicKey(examCode, entry.BlockCode.Trim(), entry.TopicNumber);
            if (!topicIndex.TryGetValue(topicKey, out SyllabusTopic? topic))
            {
                issues.Add($"{where}: no existe el tema {topicKey} en el temario.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(entry.Statement))
            {
                issues.Add($"{where}: el enunciado esta vacio.");
                continue;
            }

            List<string> options = entry.Options
                .Select(option => option?.Trim() ?? string.Empty)
                .Where(option => option.Length > 0)
                .ToList();

            if (options.Count < 2)
            {
                issues.Add($"{where}: se necesitan al menos dos opciones de respuesta.");
                continue;
            }

            if (options.Distinct(StringComparer.OrdinalIgnoreCase).Count() != options.Count)
            {
                issues.Add($"{where}: hay opciones de respuesta repetidas.");
                continue;
            }

            if (entry.CorrectIndex < 0 || entry.CorrectIndex >= options.Count)
            {
                issues.Add(
                    $"{where}: 'correctIndex' ({entry.CorrectIndex}) esta fuera del rango de opciones (0 a {options.Count - 1}).");
                continue;
            }

            if (entry.Difficulty is < 1 or > 5)
            {
                issues.Add($"{where}: la dificultad {entry.Difficulty} esta fuera del rango 1 a 5.");
                continue;
            }

            questionId++;
            var answerOptions = new List<AnswerOption>(options.Count);
            for (int i = 0; i < options.Count; i++)
            {
                answerOptions.Add(new AnswerOption(
                    Id: (questionId * 10) + i + 1,
                    SortOrder: (byte)(i + 1),
                    Text: options[i],
                    IsCorrect: i == entry.CorrectIndex));
            }

            questions.Add(new Question
            {
                Id = questionId,
                ExternalId = externalId,
                TopicId = topic.Id,
                ExamCode = examCode,
                BlockCode = topic.BlockCode,
                TopicNumber = topic.Number,
                TopicTitle = topic.Title,
                Difficulty = (Difficulty)entry.Difficulty,
                Statement = entry.Statement.Trim(),
                Explanation = string.IsNullOrWhiteSpace(entry.Explanation) ? null : entry.Explanation.Trim(),
                Source = ToSource(entry.Source),
                Tags = entry.Tags
                    .Where(tag => !string.IsNullOrWhiteSpace(tag))
                    .Select(tag => tag.Trim().ToLowerInvariant())
                    .Distinct(StringComparer.Ordinal)
                    .ToList(),
                IsActive = entry.IsActive,
                Options = answerOptions,
            });
        }

        return questions;
    }

    private T? Deserialize<T>(string path, List<string> issues)
        where T : class
    {
        try
        {
            using FileStream stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<T>(stream, SerializerOptions);
        }
        catch (JsonException ex)
        {
            issues.Add($"{Path.GetFileName(path)}: JSON invalido ({ex.Message}).");
            return null;
        }
        catch (IOException ex)
        {
            issues.Add($"{Path.GetFileName(path)}: no se ha podido leer el fichero ({ex.Message}).");
            return null;
        }
    }

    internal static string TopicKey(string examCode, string blockCode, int topicNumber) =>
        $"{examCode}|{blockCode}|{topicNumber}";

    private static OfficialSource? ToSource(SourceFile? source) =>
        source is null || string.IsNullOrWhiteSpace(source.Reference)
            ? null
            : new OfficialSource(
                source.Reference.Trim(),
                string.IsNullOrWhiteSpace(source.Publication) ? null : source.Publication.Trim(),
                string.IsNullOrWhiteSpace(source.Url) ? null : source.Url.Trim());

    private static ScoringPolicy ToScoring(ScoringFile? scoring) =>
        scoring is null
            ? ScoringPolicy.Default
            : new ScoringPolicy(
                scoring.CorrectPoints,
                scoring.IncorrectPoints,
                scoring.BlankPoints,
                scoring.MaxScore,
                scoring.PassMark);

    private static ExamFormat ToFormat(FormatFile? format) =>
        format is null
            ? ExamFormat.Default
            : new ExamFormat(
                format.QuestionCount,
                format.ReserveQuestions,
                format.DurationMinutes,
                format.OptionsPerQuestion);

    /// <summary>Genera un slug ASCII estable a partir de un titulo.</summary>
    public static string Slugify(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        string source = value.Trim().ToLowerInvariant();
        var builder = new StringBuilder(source.Length);
        bool lastWasSeparator = false;

        foreach (char character in source)
        {
            // La transliteracion se hace con una tabla explicita en lugar de con Normalize():
            // la aplicacion se compila con globalizacion invariante y ahi la normalizacion Unicode
            // no descompone los acentos, con lo que 'administracion' acabaria como 'administraci-n'.
            char folded = Fold(character);

            if (folded == '\0')
            {
                continue;
            }

            if (char.IsAsciiLetterOrDigit(folded))
            {
                builder.Append(folded);
                lastWasSeparator = false;
            }
            else if (!lastWasSeparator && builder.Length > 0)
            {
                builder.Append('-');
                lastWasSeparator = true;
            }
        }

        return builder.ToString().Trim('-');
    }

    /// <summary>
    /// Reduce un caracter a su equivalente ASCII. Devuelve <c>'\0'</c> para los signos diacriticos
    /// sueltos, que deben desaparecer sin dejar separador.
    /// </summary>
    private static char Fold(char character) => character switch
    {
        'á' or 'à' or 'ä' or 'â' or 'ã' or 'å' => 'a',
        'é' or 'è' or 'ë' or 'ê' => 'e',
        'í' or 'ì' or 'ï' or 'î' => 'i',
        'ó' or 'ò' or 'ö' or 'ô' or 'õ' => 'o',
        'ú' or 'ù' or 'ü' or 'û' => 'u',
        'ñ' => 'n',
        'ç' => 'c',
        'ý' or 'ÿ' => 'y',
        '̀' or '́' or '̂' or '̃' or '̈' or '̧' => '\0',
        _ => character,
    };
}
