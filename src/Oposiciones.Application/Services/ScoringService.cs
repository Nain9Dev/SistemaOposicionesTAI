using Oposiciones.Application.DTOs;
using Oposiciones.Application.Interfaces;

namespace Oposiciones.Application.Services;

/// <summary>
/// Baremo oficial INAP: +1,00 por acierto, -0,33 por fallo, 0,00 por respuesta en blanco.
/// </summary>
public sealed class ScoringService : IScoringService
{
    private const double PointsPerCorrect = 1.0d;
    private const double PointsPerWrong = 0.33d;
    private const double MaxGrade = 10.0d;

    public double PenaltyPerWrongAnswer => PointsPerWrong;

    public ExamScore Evaluate(int total, int correct, int wrong)
    {
        var safeTotal = Math.Max(total, 0);
        var safeCorrect = Math.Clamp(correct, 0, safeTotal);
        var safeWrong = Math.Clamp(wrong, 0, safeTotal - safeCorrect);
        var blank = safeTotal - safeCorrect - safeWrong;

        if (safeTotal == 0)
        {
            // Un examen sin preguntas no puntua: evita la division por cero que producia NaN.
            return new ExamScore(0, 0, 0, 0, 0d, 0d, 0d);
        }

        var netPoints = (safeCorrect * PointsPerCorrect) - (safeWrong * PointsPerWrong);
        var maxPoints = safeTotal * PointsPerCorrect;
        var grade = Math.Max(0d, netPoints / maxPoints * MaxGrade);

        return new ExamScore(
            safeTotal,
            safeCorrect,
            safeWrong,
            blank,
            Math.Round(netPoints, 2, MidpointRounding.AwayFromZero),
            maxPoints,
            Math.Round(grade, 2, MidpointRounding.AwayFromZero));
    }
}
