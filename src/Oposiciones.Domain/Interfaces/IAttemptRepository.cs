using System.Threading.Tasks;

namespace Oposiciones.Domain.Interfaces
{
	public interface IAttemptRepository
	{
		Task<long> StartAsync(long testId, string userName);
		Task<int> AnswerAsync(long attemptId, long questionId, long answerOptionId);
		Task<FinishAttemptResult> FinishAsync(long attemptId);
	}

	public class FinishAttemptResult
	{
		public long AttemptId { get; set; }
		public decimal? Score { get; set; }
		public System.DateTime? FinishedAt { get; set; }
	}
}