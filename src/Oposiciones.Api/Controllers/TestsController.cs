using Microsoft.AspNetCore.Mvc;
using Oposiciones.Domain.Interfaces;
using System.Threading.Tasks;
using System.Linq;

namespace Oposiciones.Api.Controllers
{
    [ApiController]
    [Route("api/tests")]
    public class TestsController : ControllerBase
    {
        private readonly ITestRepository _testRepository;

        public TestsController(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public class GenerateTestRequest
        {
            public string Title { get; set; } = string.Empty;
            public int SyllabusTopicId { get; set; }
            public byte Difficulty { get; set; }
            public int TotalQuestions { get; set; }
        }
        public class TestResponse
        {
            public long TestId { get; set; }
            public string Title { get; set; } = string.Empty;
            public List<QuestionResponse> Questions { get; set; } = new List<QuestionResponse>();
        }

        public class QuestionResponse
        {
            public long QuestionId { get; set; }
            public string Statement { get; set; } = string.Empty;
            public List<OptionResponse> Options { get; set; } = new List<OptionResponse>();
        }

        public class OptionResponse
        {
            public long Id { get; set; }
            public byte SortOrder { get; set; }
            public string Text { get; set; } = string.Empty;
        }

        [HttpGet("{testId:long}")]
        public async Task<IActionResult> GetById(long testId)
        {
            var rows = (await _testRepository.GetTestDetailRowsAsync(testId)).ToList();
            if (rows.Count == 0) return NotFound();

            var response = new TestResponse
            {
                TestId = rows[0].TestId,
                Title = rows[0].Title
            };

            var grouped = rows.GroupBy(r => new { r.QuestionId, r.Statement });
            foreach (var g in grouped)
            {
                var q = new QuestionResponse
                {
                    QuestionId = g.Key.QuestionId,
                    Statement = g.Key.Statement
                };

                foreach (var r in g)
                {
                    q.Options.Add(new OptionResponse
                    {
                        Id = r.OptionId,
                        SortOrder = r.SortOrder,
                        Text = r.OptionText
                    });
                }

                response.Questions.Add(q);
            }

            return Ok(response);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateTestRequest request)
        {
            var testId = await _testRepository.GenerateAsync(
                request.Title,
                request.SyllabusTopicId,
                request.Difficulty,
                request.TotalQuestions);

            return Ok(new { testId });
        }
    }
}