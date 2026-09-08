using Microsoft.Extensions.Logging;
using Oposiciones.Application.Common;
using Oposiciones.Application.DTOs;
using Oposiciones.Application.Interfaces;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;

namespace Oposiciones.Application.Services;

public sealed class StudyQuestionService : IStudyQuestionService
{
    /// <summary>Tamano por defecto de un simulacro cuando el cliente no indica cantidad.</summary>
    public const int DefaultQuestionCount = 20;

    /// <summary>Tope duro: protege la base de datos y el render del cliente.</summary>
    public const int MaxQuestionCount = 100;

    private readonly IStudyQuestionRepository _repository;
    private readonly ILogger<StudyQuestionService> _logger;

    public StudyQuestionService(IStudyQuestionRepository repository, ILogger<StudyQuestionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PreguntaDto>> GetPreguntasAsync(
        string? bloqueSelector,
        int cantidad,
        int? dificultad = null,
        int? temaId = null,
        CancellationToken cancellationToken = default)
    {
        var filter = BuildFilter(bloqueSelector, cantidad, dificultad, temaId);
        var questions = await _repository.GetRandomAsync(filter, cancellationToken);

        var result = new List<PreguntaDto>(questions.Count);
        foreach (var question in questions)
        {
            if (!question.IsAnswerable)
            {
                // Una pregunta sin solucion marcada corromperia la correccion: se descarta y se registra.
                _logger.LogWarning(
                    "Pregunta {QuestionId} descartada: {OptionCount} opciones, indice correcto {CorrectIndex}.",
                    question.Id, question.Options.Count, question.CorrectOptionIndex);
                continue;
            }

            result.Add(Map(question));
        }

        return result;
    }

    public Task<int> ContarDisponiblesAsync(
        string? bloqueSelector,
        int? dificultad = null,
        int? temaId = null,
        CancellationToken cancellationToken = default)
    {
        var filter = BuildFilter(bloqueSelector, MaxQuestionCount, dificultad, temaId);
        return _repository.CountAsync(filter, cancellationToken);
    }

    private static StudyQuestionFilter BuildFilter(string? bloqueSelector, int cantidad, int? dificultad, int? temaId)
    {
        var (blockId, blockCode) = BlockSelector.Parse(bloqueSelector);
        var limit = cantidad <= 0 ? DefaultQuestionCount : Math.Min(cantidad, MaxQuestionCount);
        var difficulty = dificultad is >= 1 and <= 5 ? dificultad : null;

        return new StudyQuestionFilter(blockId, blockCode, temaId, difficulty, limit);
    }

    private static PreguntaDto Map(StudyQuestion question) => new()
    {
        Id = question.Id,
        Enunciado = question.Statement,
        Opciones = question.Options.Select(option => option.Text).ToArray(),
        RespuestaCorrecta = question.CorrectOptionIndex,
        Bloque = question.BlockCode,
        BloqueId = question.BlockId,
        BloqueNombre = question.BlockName,
        TemaId = question.TopicId,
        Tema = question.TopicTitle,
        Dificultad = question.Difficulty,
        Explicacion = question.Explanation
    };
}
