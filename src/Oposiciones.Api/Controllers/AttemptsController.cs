using Microsoft.AspNetCore.Mvc;
using Oposiciones.Domain.Interfaces;
using System.Threading.Tasks;

namespace Oposiciones.Api.Controllers
{
    [ApiController]
    [Route("api/attempts")]
    public class AttemptsController : ControllerBase
    {
        private readonly IAttemptRepository _attemptRepository;

        public AttemptsController(IAttemptRepository attemptRepository)
        {
            _attemptRepository = attemptRepository;
        }

        public class StartAttemptRequest
        {
            public long TestId { get; set; }
            public string UserName { get; set; } = "demo";
        }

        public class AnswerRequest
        {
            public long QuestionId { get; set; }
            public long AnswerOptionId { get; set; }
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartAttemptRequest request)
        {
            var attemptId = await _attemptRepository.StartAsync(request.TestId, request.UserName);
            return Ok(new { attemptId });
        }

        [HttpPost("{attemptId:long}/answer")]
        public async Task<IActionResult> Answer(long attemptId, [FromBody] AnswerRequest request)
        {
            var ok = await _attemptRepository.AnswerAsync(attemptId, request.QuestionId, request.AnswerOptionId);
            return Ok(new { ok = ok == 1 });
        }

        [HttpPost("{attemptId:long}/finish")]
        public async Task<IActionResult> Finish(long attemptId)
        {
            var result = await _attemptRepository.FinishAsync(attemptId);
            return Ok(result);
        }
    }
}