using Oposiciones.Application.DTOs;

namespace Oposiciones.Application.Interfaces;

/// <summary>
/// Unica fuente de verdad del baremo. Antes convivian tres calculos distintos: el del cliente,
/// el de AttemptFinish en base de datos y el implicito al guardar el intento.
/// </summary>
public interface IScoringService
{
    /// <summary>Puntos que resta cada respuesta erronea (valor positivo).</summary>
    double PenaltyPerWrongAnswer { get; }

    /// <summary>
    /// Evalua un simulacro. Los contadores se saturan al total para tolerar entradas
    /// incoherentes procedentes del cliente sin propagar valores imposibles.
    /// </summary>
    ExamScore Evaluate(int total, int correct, int wrong);
}
