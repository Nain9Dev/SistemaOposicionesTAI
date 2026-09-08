namespace Oposiciones.Api.DTOs;

/// <summary>Parametros de generacion de un cuestionario de estudio.</summary>
public class PreguntasQueryDto
{
    /// <summary>Selector de bloque: "all", el indice numerico ("1".."4") o el codigo romano.</summary>
    public string? Bloque { get; set; }

    /// <summary>Numero de preguntas solicitadas. El servidor lo satura a su maximo permitido.</summary>
    public int Cantidad { get; set; }

    /// <summary>Dificultad exacta (1..5). Nulo acepta cualquiera.</summary>
    public int? Dificultad { get; set; }

    /// <summary>Restringe a un tema concreto del temario.</summary>
    public int? TemaId { get; set; }
}

/// <summary>Disponibilidad de preguntas para un filtro, antes de lanzar el simulacro.</summary>
public class DisponibilidadDto
{
    public string Bloque { get; set; } = string.Empty;
    public int Disponibles { get; set; }
    public int MaximoPorSimulacro { get; set; }
}

public class StartAttemptRequestDto
{
    public long TestId { get; set; }
}

public class AnswerRequestDto
{
    public long QuestionId { get; set; }

    /// <summary>Opcion elegida. Nulo marca la pregunta como no contestada (en blanco).</summary>
    public long? AnswerOptionId { get; set; }
}

public class GenerateTestRequestDto
{
    public string Title { get; set; } = string.Empty;
    public int SyllabusTopicId { get; set; }
    public byte Difficulty { get; set; }
    public int TotalQuestions { get; set; }
}

public class TestResponseDto
{
    public long TestId { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<TestQuestionDto> Questions { get; set; } = [];
}

public class TestQuestionDto
{
    public long QuestionId { get; set; }
    public string Statement { get; set; } = string.Empty;
    public List<TestOptionDto> Options { get; set; } = [];
}

/// <summary>
/// Opcion de respuesta tal y como se expone al cliente. No incluye IsCorrect de forma
/// deliberada: la correccion la resuelve el servidor al finalizar el intento.
/// </summary>
public class TestOptionDto
{
    public long Id { get; set; }
    public byte SortOrder { get; set; }
    public string Text { get; set; } = string.Empty;
}
