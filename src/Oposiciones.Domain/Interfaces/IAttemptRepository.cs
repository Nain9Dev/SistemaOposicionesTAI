namespace Oposiciones.Domain.Interfaces
{
    public interface IAttemptRepository
    {
        Task<long> StartAsync(long testId, string userName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registra o sustituye la respuesta de una pregunta. Devuelve false si la pregunta no
        /// pertenece al test del intento o si la opcion no pertenece a esa pregunta.
        /// </summary>
        Task<bool> AnswerAsync(long attemptId, long questionId, long? answerOptionId, CancellationToken cancellationToken = default);

        Task<FinishAttemptResult> FinishAsync(long attemptId, CancellationToken cancellationToken = default);

        /// <summary>Propietario del intento, o null si el intento no existe.</summary>
        Task<string?> GetOwnerAsync(long attemptId, CancellationToken cancellationToken = default);

        /// <summary>Indica si el intento ya fue cerrado, para impedir respuestas posteriores.</summary>
        Task<bool> IsFinishedAsync(long attemptId, CancellationToken cancellationToken = default);
    }

    /// <summary>Desglose del intento con el baremo oficial INAP aplicado en base de datos.</summary>
    public class FinishAttemptResult
    {
        public long AttemptId { get; set; }
        public int Correct { get; set; }
        public int Wrong { get; set; }
        public int Blank { get; set; }
        public int Total { get; set; }
        public decimal Score { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}
