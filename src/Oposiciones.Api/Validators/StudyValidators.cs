using FluentValidation;
using Oposiciones.Api.DTOs;
using Oposiciones.Application.DTOs;
using Oposiciones.Application.Services;

namespace Oposiciones.Api.Validators;

public class PreguntasQueryDtoValidator : AbstractValidator<PreguntasQueryDto>
{
    public PreguntasQueryDtoValidator()
    {
        RuleFor(x => x.Cantidad)
            .InclusiveBetween(0, StudyQuestionService.MaxQuestionCount)
            .WithMessage($"La cantidad debe estar entre 1 y {StudyQuestionService.MaxQuestionCount}. Use 0 para el valor por defecto.");

        RuleFor(x => x.Dificultad)
            .InclusiveBetween(1, 5).When(x => x.Dificultad.HasValue)
            .WithMessage("La dificultad debe estar entre 1 y 5.");

        RuleFor(x => x.TemaId)
            .GreaterThan(0).When(x => x.TemaId.HasValue)
            .WithMessage("El identificador de tema debe ser positivo.");

        RuleFor(x => x.Bloque)
            .MaximumLength(20).WithMessage("El selector de bloque no es valido.");
    }
}

public class IntentoDtoValidator : AbstractValidator<IntentoDto>
{
    public IntentoDtoValidator()
    {
        RuleFor(x => x.Total)
            .GreaterThan(0).WithMessage("El simulacro debe contener al menos una pregunta.")
            .LessThanOrEqualTo(StudyQuestionService.MaxQuestionCount)
            .WithMessage($"Un simulacro no puede superar las {StudyQuestionService.MaxQuestionCount} preguntas.");

        RuleFor(x => x.Aciertos).GreaterThanOrEqualTo(0).WithMessage("Los aciertos no pueden ser negativos.");
        RuleFor(x => x.Fallos).GreaterThanOrEqualTo(0).WithMessage("Los fallos no pueden ser negativos.");

        RuleFor(x => x)
            .Must(intento => intento.Aciertos + intento.Fallos <= intento.Total)
            .WithMessage("La suma de aciertos y fallos no puede superar el total de preguntas.");

        RuleFor(x => x.Bloque)
            .MaximumLength(20).WithMessage("El selector de bloque no es valido.");

        // Una fecha futura descuadra la tendencia y el "ultimo intento": se admite un
        // pequeno margen por desfase de reloj del cliente.
        RuleFor(x => x.Fecha)
            .LessThanOrEqualTo(_ => DateTime.UtcNow.AddMinutes(5)).When(x => x.Fecha.HasValue)
            .WithMessage("La fecha del intento no puede estar en el futuro.");
    }
}

public class StartAttemptRequestDtoValidator : AbstractValidator<StartAttemptRequestDto>
{
    public StartAttemptRequestDtoValidator()
    {
        RuleFor(x => x.TestId).GreaterThan(0).WithMessage("El identificador de test debe ser positivo.");
    }
}

public class AnswerRequestDtoValidator : AbstractValidator<AnswerRequestDto>
{
    public AnswerRequestDtoValidator()
    {
        RuleFor(x => x.QuestionId).GreaterThan(0).WithMessage("El identificador de pregunta debe ser positivo.");

        RuleFor(x => x.AnswerOptionId)
            .GreaterThan(0).When(x => x.AnswerOptionId.HasValue)
            .WithMessage("El identificador de opcion debe ser positivo. Omitalo para dejar la pregunta en blanco.");
    }
}

public class GenerateTestRequestDtoValidator : AbstractValidator<GenerateTestRequestDto>
{
    public GenerateTestRequestDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El titulo es obligatorio.")
            .MaximumLength(200).WithMessage("El titulo no puede superar los 200 caracteres.");

        RuleFor(x => x.SyllabusTopicId).GreaterThan(0).WithMessage("El tema del temario debe ser positivo.");

        RuleFor(x => x.Difficulty)
            .InclusiveBetween((byte)1, (byte)5).WithMessage("La dificultad debe estar entre 1 y 5.");

        RuleFor(x => x.TotalQuestions)
            .InclusiveBetween(1, StudyQuestionService.MaxQuestionCount)
            .WithMessage($"El numero de preguntas debe estar entre 1 y {StudyQuestionService.MaxQuestionCount}.");
    }
}
